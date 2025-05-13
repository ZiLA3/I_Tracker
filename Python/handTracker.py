import cv2 as cv
from cvzone.HandTrackingModule import HandDetector

class handTracker:
    """
    손 위치 추적 클래스
    
    CVZone의 HandDetector를 활용하여 사용자의 손 위치를 감지하고 추적하는 클래스입니다.
    감지된 손의 랜드마크 위치를 Unity로 전송하기 위한 형식으로 변환합니다.
    """
    def __init__(self, camera=cv.VideoCapture(0), xyMax=(1920, 1080)):
        """
        손 트래커 초기화
        
        Args:
            camera (cv.VideoCapture): 비디오 입력 소스 (기본값: 기본 카메라)
            xyMax (tuple): 화면 해상도 (기본값: 1920x1080)
        """
        self.handDetector = HandDetector(maxHands=1, detectionCon=0.8)  # CVZone 손 감지기 초기화 (최대 1개 손, 감지 신뢰도 0.8)
        self.camera = camera  # 카메라 객체
        self.yMax = xyMax[1]  # 화면 세로 해상도 (Y축 최대값)

    def get_hand_position(self):
        """
        현재 손 위치를 감지하고 반환
        
        손 감지 과정:
        1. 카메라에서 이미지 캡처
        2. HandDetector로 손 랜드마크 감지
        3. 감지된 랜드마크 좌표를 Unity에서 사용 가능한 형식으로 변환
           - Y축 좌표는 화면 좌표계와 일치하도록 반전 (상단이 0)
        
        Returns:
            tuple: (hand_position, image)
                - hand_position (list): 손 랜드마크 좌표 배열 [x1,y1,z1,x2,y2,z2,...], 감지 실패 시 None
                  각 랜드마크는 x, y, z 좌표로 구성 (총 21개 랜드마크, 63개 값)
                - image (numpy.ndarray): 처리된 이미지 (손 랜드마크 표시)
        """
        success, image = self.camera.read()  # 카메라에서 프레임 읽기
        if not success:
            return None, None  # 카메라 읽기 실패 시 None 반환
        
        # CVZone 손 감지기로 손 위치 감지
        hands, image = self.handDetector.findHands(image, flipType=True)  # 이미지 좌우 반전하여 손 감지
        
        hand_position = None  # 기본값을 None으로 설정
        if hands:  # 손이 감지된 경우
            lmList = hands[0]['lmList']  # 첫 번째 감지된 손의 랜드마크 리스트
            hand_position = []
            for x, y, z in lmList:  # 각 랜드마크 좌표 처리
                # x 좌표는 그대로, y 좌표는 반전 (화면 좌표계에 맞춤), z 좌표는 그대로
                hand_position.extend([x, self.yMax - y, z])
        
        return hand_position, image  # 변환된 손 위치 데이터와 처리된 이미지 반환