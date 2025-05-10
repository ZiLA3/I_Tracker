import cv2 as cv
import asyncio
import json
import time
from xyMapper import xyMapper
from irisTracker import irisTracker
from UDPManager import UDPManager

# 전역 변수
running = True
paused = False
calibration_mode = True
calibration_points = {}
screen_size = (1920, 1080)  # 기본값, 유니티에서 실제 값 수신

# 비동기 메인 함수
async def main():
    global running, paused, calibration_mode, calibration_points, screen_size
    
    # UDP 소켓 초기화
    udp = UDPManager()
    
    # 홍채 트래커 초기화
    iris = irisTracker()
    mapper = None
    
    # 유니티에 시작 신호 전송
    udp.send({"type": "start", "screen_size": screen_size})
    
    # 콜백 함수 등록
    def on_screen_size(message):
        global screen_size
        screen_size = (message['width'], message['height'])
        print(f"화면 크기 수신: {screen_size}")
        return True
    
    def on_calibration_request(message):
        corner = message.get('corner')
        print(f"캘리브레이션 요청: {corner} 코너를 바라봐주세요.")
        return True
    
    def on_captured(message):
        global calibration_points
        corner = message.get('corner')
        iris_pos = message.get('iris_pos')
        if corner and iris_pos:
            calibration_points[corner] = iris_pos
            print(f"{corner} 위치 캡처됨: {iris_pos}")
        return True
    
    def on_pause(message):
        global paused
        paused = True
        print("일시정지됨")
        return True
    
    def on_resume(message):
        global paused
        paused = False
        print("재개됨")
        return True
    
    def on_kill(message):
        global running
        running = False
        print("종료 신호 수신")
        return False  # 수신 루프 종료
    
    # 콜백 등록
    udp.register_callback('screen_size', on_screen_size)
    udp.register_callback('calibration_request', on_calibration_request)
    udp.register_callback('captured', on_captured)
    udp.register_callback('pause', on_pause)
    udp.register_callback('resume', on_resume)
    udp.register_callback('kill', on_kill)
    
    # 비동기 리스너 시작
    listener_task = asyncio.create_task(udp.start_listening())
    
    try:
        # 메인 처리 루프
        while running:
            if paused:
                # 일시정지 상태에서 키 입력 대기
                key = cv.waitKey(100) & 0xFF
                if key == 112:  # 'p' 키로 재개
                    paused = False
                    udp.send({"type": "resume"})
                # CPU 사용량 감소를 위한 짧은 대기
                await asyncio.sleep(0.01)
                continue
            
            # 홍채 위치 가져오기
            iris_position, image = iris.get_iris_position()
            
            if iris_position and image is not None:
                if calibration_mode:
                    # 캘리브레이션 모드
                    cv.putText(image, "Calibration Mode", (10, 30), 
                              cv.FONT_HERSHEY_SIMPLEX, 0.7, (0, 0, 255), 2)
                    
                    # 현재 캘리브레이션 상태 표시
                    cv.putText(image, f"Points: {len(calibration_points)}/4", 
                              (10, 60), cv.FONT_HERSHEY_SIMPLEX, 0.7, (0, 0, 255), 2)
                    
                    # 유니티에 현재 홍채 위치 전송 (캡처를 위해)
                    udp.send({
                        "type": "iris_position",
                        "position": iris_position
                    })
                    
                    # 캘리브레이션 포인트가 4개 모두 수집되면 xyMapper 생성
                    if len(calibration_points) == 4:
                        mapper = xyMapper(
                            calibration_points.get("lt", (0, 0)),             # 좌상단
                            calibration_points.get("lb", (0, screen_size[1])), # 좌하단
                            calibration_points.get("rt", (screen_size[0], 0)), # 우상단
                            calibration_points.get("rb", (screen_size[0], screen_size[1])), # 우하단
                            screen_size
                        )
                        
                        # 캘리브레이션 완료
                        calibration_mode = False
                        udp.send({"type": "calibration_complete"})
                        print("캘리브레이션 완료, 트래킹 시작")
                
                else:
                    # 트래킹 모드
                    if mapper:
                        # 홍채 위치 매핑
                        screen_pos = mapper.map_coordinates(iris_position)
                        
                        # Unity로 좌표 전송
                        udp.send({
                            "type": "position",
                            "screen_pos": screen_pos
                        })
                        
                        # 화면에 매핑된 좌표 표시
                        cv.putText(image, f"Screen: {screen_pos[0]:.1f}, {screen_pos[1]:.1f}", 
                                  (10, 30), cv.FONT_HERSHEY_SIMPLEX, 0.7, (0, 255, 0), 2)
            
            # 상태 표시와 화면 업데이트
            if image is not None:
                status = "Calibrating" if calibration_mode else "Tracking"
                if paused:
                    status = "Paused"
                cv.putText(image, status, (10, image.shape[0] - 20), 
                          cv.FONT_HERSHEY_SIMPLEX, 0.7, (255, 0, 0), 2)
                
                # 화면 표시
                cv.imshow('I-Tracker', image)
            
            # 키 입력 처리
            key = cv.waitKey(5) & 0xFF
            if key == 27:  # ESC
                running = False
            elif key == 112:  # 'p' (일시정지)
                paused = True
                udp.send({"type": "pause"})
            
            # 비동기 처리를 위해 짧은 대기
            await asyncio.sleep(0.01)
    
    finally:
        # 종료 신호 전송 및 정리
        udp.send({"type": "kill"})
        iris.release()
        cv.destroyAllWindows()
        udp.close()
        # 리스너 태스크 취소
        if not listener_task.done():
            listener_task.cancel()
            try:
                await listener_task
            except asyncio.CancelledError:
                pass

# 비동기 실행
if __name__ == "__main__":
    try:
        asyncio.run(main())
    except KeyboardInterrupt:
        print("프로그램이 사용자에 의해 중단되었습니다.")
    finally:
        cv.destroyAllWindows()