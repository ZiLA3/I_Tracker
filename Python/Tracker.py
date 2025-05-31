import mediapipe as mp
import cv2 as cv

LEFT_EYE_MARK, RIGHT_EYE_MARK = 468, 473 # MediaPipe Landmark Index: 468(왼쪽 홍채), 473(오른쪽 홍채)

MP_FACE_MESH = mp.solutions.face_mesh  # MediaPipe Face Mesh 모듈
FACE_MESH = MP_FACE_MESH.FaceMesh(
        static_image_mode=False,  # 비디오 스트림 모드 (False: 연속 프레임 처리)
        max_num_faces=1,  # 감지할 최대 얼굴 수
        refine_landmarks=True,  # 홍채 랜드마크 정교화 활성화
        min_detection_confidence=0.5,  # 감지 신뢰도 임계값
        min_tracking_confidence=0.5  # 추적 신뢰도 임계값
)

MP_HANDS = mp.solutions.hands
HANDS = MP_HANDS.Hands(
    max_num_hands=1,
    min_detection_confidence=0.8,
    min_tracking_confidence=0.5
)

MP_DRAW = mp.solutions.drawing_utils

class Tracker:
    def __init__(self):
        self.face_mesh = FACE_MESH
        self.hands = HANDS
        self.height, self.width = 0, 0

    def set_image_size(self, img):
        self.height, self.width, _ = img.shape

    def get_iris_pos(self, multi_face_landmarks):
        cx, cy = -1, -1
        for face_landmarks in multi_face_landmarks:
            h, w = self.height, self.width  # 이미지 크기

            left_iris = face_landmarks.landmark[LEFT_EYE_MARK]  # 왼쪽 홍채
            clx, cly = (int(left_iris.x * w), int(left_iris.y * h))  # 왼쪽 홍채 x, y좌표 (픽셀)

            right_iris = face_landmarks.landmark[RIGHT_EYE_MARK]  # 오른쪽 홍채
            crx, cry = ((right_iris.x * w), (right_iris.y * h))  # 오른쪽 홍채 x, y좌표 (픽셀)

            # 양쪽 홍채 중심의 평균 위치 계산
            cx, cy = ((clx + crx) // 2, (cly + cry) // 2)  # 중심 x, y좌표

        return [cx, cy]

    def get_hand_pos(self, multi_hand_landmarks):
        h, w = self.height, self.width
        hand_position = []
        for hand_landmarks in multi_hand_landmarks[0].landmark:
            x, y, z= (int(hand_landmarks.x * w), int(hand_landmarks.y * h), int(hand_landmarks.z * 100)) # z는 정규화된 상대 깊이 값

            hand_position.append([x, h - y, z])

        return hand_position

    def debug(self, iris_pos, hand_pos):
        print(f"{iris_pos}##{hand_pos}")

    def process(self, image_bgr, debug=False):
        if self.height == 0 or self.width == 0:
            self.set_image_size(image_bgr)

        iris_pos, hand_pos = [-1, -1], [[-1, -1, -1]]

        image_rgb = cv.cvtColor(image_bgr, cv.COLOR_BGR2RGB)  # BGR → RGB 변환

        iris_results = self.face_mesh.process(image_rgb)
        hand_results = self.hands.process(image_rgb)

        iris_multi_landmarks = iris_results.multi_face_landmarks
        hands_multi_landmarks = hand_results.multi_hand_landmarks

        if iris_multi_landmarks:
            iris_pos = self.get_iris_pos(iris_multi_landmarks)
        if hands_multi_landmarks:
            hand_pos = self.get_hand_pos(hands_multi_landmarks)

        if debug:
            self.debug(iris_pos, hand_pos)

        return iris_pos, hand_pos

if __name__ == "__main__":
    tracker = Tracker()
    cap = cv.VideoCapture(0)

    success, image = cap.read()
    if not success:
        print("Error: VideoCapture read failed.")
        exit()
    tracker.set_image_size(image)

    while True:
        success, image = cap.read()
        tracker.process(image, True)
        cv.imshow("Tracker", image)
        if cv.waitKey(1) & 0xFF == ord('q'):
            break
    cv.destroyAllWindows()