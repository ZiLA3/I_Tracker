import cv2 as cv
import socket
import json
import time
import threading
from xyMapper import xyMapper
from irisTracker import irisTracker

# Unity와의 UDP 통신 설정
UDP_IP = "127.0.0.1"  # localhost
UDP_PORT_SEND = 5000  # 유니티로 전송할 포트
UDP_PORT_RECEIVE = 5001  # 유니티에서 수신할 포트

# 송신용 소켓
send_sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)

# 수신용 소켓
receive_sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
receive_sock.bind((UDP_IP, UDP_PORT_RECEIVE))
receive_sock.settimeout(0.1)  # 블로킹 방지를 위한 타임아웃 설정

# 화면 크기 설정 (예시, 실제 화면 크기는 유니티에서 전달받음)
screen_size = (1920, 1080)

# 프로그램 상태 변수
running = True
paused = False
calibration_mode = True
calibration_points = {}

# MediaPipe를 이용한 홍채 좌표 인식 클래스

# Unity에 메시지 전송 함수
def send_to_unity(data):
    message = json.dumps(data).encode('utf-8')
    send_sock.sendto(message, (UDP_IP, UDP_PORT_SEND))

# Unity에서 메시지 수신 함수
def receive_from_unity():
    global running, paused, calibration_mode, calibration_points, screen_size
    
    try:
        data, _ = receive_sock.recvfrom(1024)
        message = json.loads(data.decode('utf-8'))
        message_type = message.get('type')
        
        if message_type == 'screen_size':
            # 화면 크기 정보 수신
            screen_size = (message['width'], message['height'])
            print(f"화면 크기 수신: {screen_size}")
            return True
        
        elif message_type == 'calibration_request':
            # 캘리브레이션 요청 수신
            corner = message.get('corner')
            print(f"캘리브레이션 요청: {corner} 코너를 바라봐주세요.")
            return True
        
        elif message_type == 'captured':
            # 캡처 신호 수신 (유니티에서 캡처 버튼 클릭 시)
            corner = message.get('corner')
            iris_pos = message.get('iris_pos')
            if corner and iris_pos:
                calibration_points[corner] = iris_pos
                print(f"{corner} 위치 캡처됨: {iris_pos}")
            return True
        
        elif message_type == 'pause':
            # 일시정지 신호 수신
            paused = True
            print("일시정지됨")
            return True
        
        elif message_type == 'resume':
            # 재개 신호 수신
            paused = False
            print("재개됨")
            return True
        
        elif message_type == 'kill':
            # 종료 신호 수신
            running = False
            print("종료 신호 수신")
            return True
        
        return False
    
    except socket.timeout:
        # 타임아웃 시 무시
        return False
    except Exception as e:
        print(f"메시지 수신 오류: {e}")
        return False

# 유니티 메시지 수신 스레드
def receive_thread():
    global running
    
    while running:
        receive_from_unity()
        time.sleep(0.01)  # CPU 사용량 감소

# 메인 함수
def main():
    global running, paused, calibration_mode, calibration_points
    
    # 홍채 트래커 초기화
    tracker = irisTracker()
    mapper = None
    
    # 유니티에 시작 신호 전송
    send_to_unity({"type": "start", "screen_size": screen_size})
    
    # 메시지 수신 스레드 시작
    thread = threading.Thread(target=receive_thread, daemon=True)
    thread.start()
    
    try:
        while running:
            if paused:
                # 일시정지 상태에서 키 입력 대기
                key = cv.waitKey(100) & 0xFF
                if key == 112:  # 'p' 키로 재개
                    paused = False
                    send_to_unity({"type": "resume"})
                continue
            
            # 홍채 위치 가져오기
            iris_position, image = tracker.get_iris_position()
            
            if iris_position and image is not None:
                if calibration_mode:
                    # 캘리브레이션 모드
                    # Unity로부터 captured 시그널을 기다림
                    cv.putText(image, "Calibration Mode", (10, 30), 
                              cv.FONT_HERSHEY_SIMPLEX, 0.7, (0, 0, 255), 2)
                    
                    # 현재 캘리브레이션 상태 표시
                    cv.putText(image, f"Points: {len(calibration_points)}/4", 
                              (10, 60), cv.FONT_HERSHEY_SIMPLEX, 0.7, (0, 0, 255), 2)
                    
                    # 유니티에 현재 홍채 위치 전송 (캡처를 위해)
                    send_to_unity({
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
                        send_to_unity({"type": "calibration_complete"})
                        print("캘리브레이션 완료, 트래킹 시작")
                
                else:
                    # 트래킹 모드
                    if mapper:
                        # 홍채 위치 매핑
                        screen_pos = mapper.map_coordinates(iris_position)
                        
                        # Unity로 좌표 전송
                        send_to_unity({
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
                send_to_unity({"type": "pause"})
    
    finally:
        # 종료 신호 전송 및 정리
        send_to_unity({"type": "kill"})
        tracker.release()
        cv.destroyAllWindows()

if __name__ == "__main__":
    main()