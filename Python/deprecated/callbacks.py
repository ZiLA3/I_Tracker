# 콜백 함수 모음
# 메시지 파싱 처리를 담당하는 콜백 함수들입니다.

"""
UDP 통신에서 사용되는 콜백 함수 모듈

이 모듈은 UDP 통신을 통해 수신된 메시지를 처리하기 위한 콜백 함수들을 정의합니다.
모든 콜백 함수는 메시지 내용을 파싱하고 적절한 액션을 수행합니다.
전역 변수를 통해 메인 프로그램과 데이터를 공유합니다.
"""

def on_screen_size(message, callback_args=None):
    """
    화면 크기 정보를 수신하는 콜백 함수
    
    유니티 앱에서 보낸 화면 크기 정보를 파싱하여 전역 변수에 저장합니다.
    
    Args:
        message (str): 수신된 메시지 문자열 형식: "width,height"
        callback_args: 추가 콜백 인자 (사용되지 않음)
    
    Returns:
        bool: 성공 시 True, 실패 시 False
        
    영향을 주는 전역 변수:
        screen_size (tuple): 화면 크기 (너비, 높이)
    """
    global screen_size

    try:
        # 메시지 파싱: "width,height" 형식의 문자열에서 x, y 값 추출
        x, y = message.split(',')  # 쉼표로 구분된 값을 분리
        screen_size = (x, y)  # 화면 크기 업데이트
        print(f"화면 크기 수신: {screen_size}")  # 로그 출력
        return True  # 성공 반환
    except Exception as e:
        print(f"(콜백)화면크기 수신 중 오류: {e}")  # 오류 메시지 출력
        return False  # 실패 반환
    
def on_captured(message, pos):
    """
    캘리브레이션 캡처 신호 처리 콜백 함수
    
    유니티에서 보낸 캘리브레이션 포인트 캡처 명령을 처리합니다.
    현재 홍채 위치를 해당하는 캘리브레이션 포인트에 저장합니다.
    
    Args:
        message (str): 캡처 신호 문자열
            - '0': 좌상단(lt) 포인트 캡처
            - '1': 우상단(rt) 포인트 캡처
            - '2': 좌하단(lb) 포인트 캡처
            - '3': 우하단(rb) 포인트 캡처
            - '4': 캘리브레이션 완료, 평균값 계산
        pos (tuple): 현재 홍채 위치 (x, y)
        
    Returns:
        bool: 성공 시 True, 실패 시 False
        
    영향을 주는 전역 변수:
        calibration_points (dict): 캘리브레이션 포인트 데이터
        capture_mode (bool): 캘리브레이션 모드 상태
    """
    global calibration_points, capture_mode

    try:
        # 메시지 값에 따른 캘리브레이션 포인트 처리
        if message == '0':
            # 좌상단(lt) 포인트에 현재 홍채 위치 추가
            calibration_points['lt'].append(pos)
        elif message == '1':
            # 우상단(rt) 포인트에 현재 홍채 위치 추가
            calibration_points['rt'].append(pos)
        elif message == '2':
            # 좌하단(lb) 포인트에 현재 홍채 위치 추가
            calibration_points['lb'].append(pos)
        elif message == '3':
            # 우하단(rb) 포인트에 현재 홍채 위치 추가
            calibration_points['rb'].append(pos)
        elif message == '4':  # 캘리브레이션 완료
            # 각 포인트의 평균값 계산
            calibration_points['lt'] = sum(calibration_points['lt']) / len(calibration_points['lt'])
            calibration_points['rt'] = sum(calibration_points['rt']) / len(calibration_points['rt'])
            calibration_points['lb'] = sum(calibration_points['lb']) / len(calibration_points['lb'])
            calibration_points['rb'] = sum(calibration_points['rb']) / len(calibration_points['rb'])
            capture_mode = False  # 캘리브레이션 모드 종료

        return True  # 성공 반환
    except Exception as e:
        print(f"(콜백)캘리 중 오류: {e}")  # 오류 메시지 출력
        return False  # 실패 반환