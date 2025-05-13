import cv2 as cv
import time
import callbacks
import UDPManager
import handTracker
import irisTracker
import xyMapper

# 상수 선언
CAM_INT = 0  # 카메라 번호 (0: 기본 카메라, 1,2,...: 추가 카메라)
IP = "127.0.0.1"  # localhost 주소
SEND_PORT = 5000  # 데이터 전송용 UDP 포트
RECEIVE_PORT = 5001  # 데이터 수신용 UDP 포트
DEBUG = True  # 디버그 모드 활성화 여부 (True: 디버깅 정보 표시, False: 숨김)

# 굳이 보내는거 말고도 양방향으로 한 포트로 통신할 수 있을까?

# 전역 변수
capture_mode = True  # 캘리브레이션 모드 활성화 여부 (True: 캘리브레이션 중, False: 트래킹 중)
calibration_points = {'lt': [], 'rt': [], 'lb': [], 'rb': []}  # 캘리브레이션 좌표 (lt: 좌상단, rt: 우상단, lb: 좌하단, rb: 우하단)
# 받아서 평균값을 다시 계산함
screen_size = (1920, 1080)  # 기본 화면 크기, 유니티에서 실제 값 수신

def main():
    """
    메인 함수: 프로그램의 주요 실행 흐름 관리
    
    1. UDP 통신 초기화 및 콜백 설정
    2. 캘리브레이션 단계: 트래커 초기화 및 화면 모서리 캘리브레이션
    3. 실행 단계: 홍채 및 손 위치 추적, 데이터 전송
    """
    global capture_mode, calibration_points, screen_size
    
    # UDP 소켓 초기화
    udp = UDPManager.UDPManager(IP, SEND_PORT, RECEIVE_PORT)  # UDP 통신 관리자 생성
    # 콜백 등록
    udp.register_callback('screen_size', callbacks.on_screen_size)  # 화면 크기 수신 콜백
    udp.register_callback('captured', callbacks.on_captured)  # 캘리브레이션 포인트 캡처 콜백

    # Start State
    udp.receive("screen_size")  # 스크린 사이즈 수신 대기 (동기 방식)

    # Wait State (캘리브레이션 단계)
    # 트래커 초기화
    camera = cv.VideoCapture(CAM_INT)  # 카메라 초기화
    iris = irisTracker.irisTracker(camera, DEBUG)  # 홍채 트래커 초기화
    hand = handTracker.handTracker(camera, screen_size)  # 손 트래커 초기화
    mapper = None  # 좌표 매핑 객체 (캘리브레이션 후 초기화)
    
    # 캘리브레이션 모드 동작
    while capture_mode:
        # 홍채 위치 가져오기
        pos, image = iris.get_iris_position()  # 현재 홍채 위치 및 이미지
        udp.receive("captured", pos)  # 캘리브레이션 신호 수신 및 처리
        
    # 캘리브레이션 완료 후 좌표 매퍼 초기화
    mapper = xyMapper.xyMapper(
        calibration_points["lt"],  # 좌상단 캘리브레이션 포인트
        calibration_points["rt"],  # 우상단 캘리브레이션 포인트
        calibration_points["lb"],  # 좌하단 캘리브레이션 포인트
        calibration_points["rb"],  # 우하단 캘리브레이션 포인트
        screen_size  # 화면 크기
    )
    print("캘리브레이션 완료, 트래킹 시작")

    # Running State (메인 트래킹 루프)
    try:
        while True:
            # 홍채 위치 매핑
            iris_pos, image = iris.get_iris_position()  # 홍채 위치 감지
            iris_pos = mapper.map_coordinates(iris_pos)  # 화면 좌표로 매핑
            
            # 손 위치 감지
            hand_pos, image = hand.get_hand_position()  # 손 포인트 감지
            
            # Unity로 좌표 전송: iris, hand
            # 데이터그램 형식: 1,1##1,1,1/1,1,1/1,1,1/...
            # 홍채 좌표 문자열 생성 (x,y 형식)
            iris_str = f"{iris_pos[0]},{iris_pos[1]}" if iris_pos else "-1,-1"  # 홍채가 감지되지 않으면 -1,-1
            
            if hand_pos:
                hand_parts = []
                # 3개 값씩 그룹화하여 x,y,z 형태로 변환
                for i in range(0, len(hand_pos), 3):
                    if i+2 < len(hand_pos):  # 인덱스 범위 체크
                        hand_parts.append(f"{hand_pos[i]},{hand_pos[i+1]},{hand_pos[i+2]}")
                hand_str = "/".join(hand_parts)  # 손 관절 위치를 슬래시(/)로 구분
            else:
                hand_str = "-1,-1,-1"  # 손이 감지되지 않은 경우 기본값
            
            # 최종 데이터그램 생성 및 전송
            dg = iris_str + "##" + hand_str  # 홍채와 손 데이터를 ##로 구분
            udp.send(dg)  # 데이터그램 전송
            
            # 디버그 모드일 경우 화면에 정보 표시
            if DEBUG:
                # 화면에 매핑된 iris 좌표 표시
                cv.putText(image, f"Screen: {iris_str}", 
                            (10, 30), cv.FONT_HERSHEY_SIMPLEX, 0.7, (0, 255, 0), 2)
                cv.imshow('Tracker', image)  # 트래킹 화면 표시
                
            time.sleep(0.01)  # 프레임 제한 (약 100fps)
    
    finally:
        # 자원 해제
        camera.release()  # 카메라 해제
        cv.destroyAllWindows()  # 열린 창 닫기
        udp.close()  # UDP 소켓 닫기

if __name__ == "__main__":
    try:
        main()  # 메인 함수 실행
    except KeyboardInterrupt:
        print("프로그램이 사용자에 의해 중단되었습니다.")  # Ctrl+C로 종료 시 메시지 출력
    finally:
        cv.destroyAllWindows()  # 열린 창 닫기 (예외 발생 시에도 실행)