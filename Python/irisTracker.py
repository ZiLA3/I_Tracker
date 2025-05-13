import mediapipe as mp
import cv2 as cv


class irisTracker:
    """
    홍채 위치 추적 클래스
    
    MediaPipe Face Mesh를 사용하여 사용자의 얼굴 랜드마크를 감지하고
    특히 홍채의 중심 위치를 추적하는 클래스입니다.
    """

    def __init__(self, camera=cv.VideoCapture(0), debug=False):
        """
        홍채 트래커 초기화
        
        Args:
            camera (cv.VideoCapture): 비디오 입력 소스 (기본값: 기본 카메라)
            debug (bool): 디버그 모드 활성화 여부 (기본값: False)
        """
        self.mp_face_mesh = mp.solutions.face_mesh  # MediaPipe Face Mesh 모듈
        self.face_mesh = self.mp_face_mesh.FaceMesh(
            static_image_mode=False,  # 비디오 스트림 모드 (False: 연속 프레임 처리)
            max_num_faces=1,  # 감지할 최대 얼굴 수
            refine_landmarks=True,  # 홍채 랜드마크 정교화 활성화
            min_detection_confidence=0.5,  # 감지 신뢰도 임계값
            min_tracking_confidence=0.5  # 추적 신뢰도 임계값
        )
        self.camera = camera  # 카메라 객체
        self.debug = debug  # 디버그 모드 설정

    def get_iris_position(self):
        """
        현재 홍채 위치를 감지하고 반환
        
        홍채 감지 과정:
        1. 카메라에서 이미지 캡처
        2. MediaPipe Face Mesh로 얼굴 랜드마크 감지
        3. 양쪽 홍채 랜드마크 중심 추출 (468, 473)
        4. 두 홍채의 평균 중심 위치 계산
        
        Returns:
            tuple: (iris_position, image)
                - iris_position (tuple): 홍채 중심 좌표 (x, y), 감지 실패 시 None
                - image (numpy.ndarray): 처리된 이미지 (디버그 모드일 경우 시각화 요소 포함)
        """
        success, image = self.camera.read()  # 카메라에서 프레임 읽기
        image = cv.flip(image, 1)  # 이미지 좌우 반전 (거울 모드)
        if not success:
            return None, None  # 카메라 읽기 실패 시 None 반환

        # MediaPipe는 RGB 이미지 입력 필요
        image_rgb = cv.cvtColor(image, cv.COLOR_BGR2RGB)  # BGR → RGB 변환
        results = self.face_mesh.process(image_rgb)  # Face Mesh 모델로 얼굴 랜드마크 분석

        iris_position = None  # 기본값 None으로 설정

        # 얼굴 랜드마크가 감지된 경우
        if results.multi_face_landmarks:
            for face_landmarks in results.multi_face_landmarks:
                h, w, _ = image.shape  # 이미지 크기

                # 왼쪽, 오른쪽 홍채 중심 landmark: 468, 473
                # MediaPipe 랜드마크 인덱스: 468(왼쪽 홍채), 473(오른쪽 홍채)
                left_iris = face_landmarks.landmark[468]  # 왼쪽 홍채 랜드마크
                clx = int(left_iris.x * w)  # 왼쪽 홍채 x좌표 (픽셀)
                cly = int(left_iris.y * h)  # 왼쪽 홍채 y좌표 (픽셀)

                right_iris = face_landmarks.landmark[473]  # 오른쪽 홍채 랜드마크
                crx = int(right_iris.x * w)  # 오른쪽 홍채 x좌표 (픽셀)
                cry = int(right_iris.y * h)  # 오른쪽 홍채 y좌표 (픽셀)

                # 양쪽 홍채 중심의 평균 위치 계산
                cx = (clx + crx) // 2  # 중심 x좌표
                cy = (cly + cry) // 2  # 중심 y좌표

                # 디버그 모드일 경우 시각화 요소 추가
                if self.debug:
                    # 왼쪽 홍채 위치 표시 (녹색)
                    cv.circle(image, (clx, cly), 3, (0, 255, 0), -1)
                    # 오른쪽 홍채 위치 표시 (녹색)
                    cv.circle(image, (crx, cry), 3, (0, 255, 0), -1)
                    # 평균 홍채 중심 표시 (빨간색)
                    cv.circle(image, (cx, cy), 3, (0, 0, 255), -1)

                iris_position = (cx, cy)  # 최종 홍채 위치

        return iris_position, image  # 홍채 위치와 처리된 이미지 반환
