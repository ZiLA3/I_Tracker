import cv2 as cv
import asyncio
import threading
import callbacks
from UDPManager import UDPManager
from handTracker import handTracker
from irisTracker import irisTracker
from xyMapper import xyMapper

# 상수 선언
CAM_INT = 0  # 카메라 번호
IP = "127.0.0.1"  # localhost
SEND_PORT = 5000
RECEIVE_PORT = 5001

# 굳이 보내는거 말고도 양방향으로 한 포트로 통신할 수 있을까?

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
    udp = UDPManager(IP, SEND_PORT, RECEIVE_PORT)
    # 콜백 등록
    udp.register_callback('screen_size', callbacks.on_screen_size)
    udp.register_callback('captured', callbacks.on_captured)
    udp.register_callback('pause', callbacks.on_pause)
    udp.register_callback('resume', callbacks.on_resume)
    udp.register_callback('kill', callbacks.on_kill)
    # 유니티에 시작 신호 전송
    udp.send({"type": "start"})
    udp.receive(message_type='screen_size')  # 스크린 사이즈 기다림 (동기)

    # 트래커 초기화
    camera = cv.VideoCapture(CAM_INT)
    iris = irisTracker(camera)
    hand = handTracker(camera, screen_size)

    # 캘리 모드 플래그로 캘리 시도
    mapper = None
    while calibration_mode:
        # 홍채 위치 가져오기
        iris_position_origin, image = iris.get_iris_position()

        if iris_position_origin and image is not None:
            # 캘리브레이션 모드
            cv.putText(image, "Calibration Mode", (10, 30),
                       cv.FONT_HERSHEY_SIMPLEX, 0.7, (0, 0, 255), 2)

            # 현재 캘리브레이션 상태 표시
            cv.putText(image, f"Points: {len(calibration_points)}/4",
                       (10, 60), cv.FONT_HERSHEY_SIMPLEX, 0.7, (0, 0, 255), 2)

            # 유니티에 현재 홍채 위치 전송 (캡처를 위해)
            udp.send({
                "type": "iris_position_origin",
                "position": iris_position_origin
            })

            # 캘리브레이션 포인트가 4개 모두 수집되면 xyMapper 생성
            if len(calibration_points) == 4:
                mapper = xyMapper(
                    calibration_points.get(
                        "lt", (0, 0)),                           # 좌상단
                    calibration_points.get(
                        "lb", (0, screen_size[1])),              # 좌하단
                    calibration_points.get(
                        "rt", (screen_size[0], 0)),              # 우상단
                    calibration_points.get(
                        "rb", (screen_size[0], screen_size[1])),  # 우하단
                    screen_size
                )

                # 캘리브레이션 완료
                calibration_mode = False
                udp.send({"type": "calibration_complete"})
                print("캘리브레이션 완료, 트래킹 시작")

    # UDP Listening 시작 (멀티 쓰레딩)
    threading.Thread(target=udp.start_listening, daemon=True).start()
    try:
        # 메인 처리 루프
        while running:
            if paused:
                # CPU 사용량 감소를 위한 짧은 대기
                await asyncio.sleep(0.01)
                continue
            else:
                # 홍채 위치 매핑
                iris_position_origin, image = iris.get_iris_position()
                iris_position = mapper.map_coordinates(iris_position_origin)
                # 손 위치
                hand_position, image = hand.get_hand_position()

                # Unity로 좌표 전송: iris, hand
                udp.send({
                    "type": "iris_position",
                    "iris_pos": iris_position,
                    "hand_pos": hand_position
                })

                # 화면에 매핑된 iris 좌표 표시
                cv.putText(image, f"Screen: {iris_position[0]:.1f}, {iris_position[1]:.1f}",
                           (10, 30), cv.FONT_HERSHEY_SIMPLEX, 0.7, (0, 255, 0), 2)

                # 상태 표시와 화면 업데이트
                if image is not None:
                    status = "Calibrating" if calibration_mode else "Tracking"
                    if paused:
                        status = "Paused"
                    cv.putText(image, status, (10, image.shape[0] - 20),
                               cv.FONT_HERSHEY_SIMPLEX, 0.7, (255, 0, 0), 2)

                    # 화면 표시 (디버깅용)
                    cv.imshow('I-Tracker', image)

                # 비동기 처리를 위해 짧은 대기
                await asyncio.sleep(0.01)

    finally:
        # 종료 완료 신호 전송 및 정리
        udp.send({"type": "kill"})
        camera.release()
        cv.destroyAllWindows()
        udp.close()

# 비동기 실행
if __name__ == "__main__":
    try:
        asyncio.run(main())
    except KeyboardInterrupt:
        print("프로그램이 사용자에 의해 중단되었습니다.")
    finally:
        cv.destroyAllWindows()
