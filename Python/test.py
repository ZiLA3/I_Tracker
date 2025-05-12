import cv2
import mediapipe as mp
import numpy as np

# 1) MediaPipe Face Mesh 초기화
mp_face_mesh = mp.solutions.face_mesh
face_mesh = mp_face_mesh.FaceMesh(
    static_image_mode=False,
    max_num_faces=1,
    refine_landmarks=True,
    min_detection_confidence=0.5,
    min_tracking_confidence=0.5
)

# 2) 눈 외곽 인덱스 정의
LEFT_EYE_OUTER = [33, 173]
RIGHT_EYE_OUTER = [263, 398]

# 3) 웹캠 열기
cap = cv2.VideoCapture(0)
if not cap.isOpened():
    print("카메라를 열 수 없습니다.")
    exit()

while True:
    ret, frame = cap.read()
    if not ret:
        break

    # BGR → RGB
    rgb = cv2.cvtColor(frame, cv2.COLOR_BGR2RGB)
    results = face_mesh.process(rgb)

    if results.multi_face_landmarks:
        h, w, _ = frame.shape
        for face_landmarks in results.multi_face_landmarks:
            # 1) 랜드마크를 픽셀 좌표로 변환
            pts = [(int(lm.x * w), int(lm.y * h))
                   for lm in face_landmarks.landmark]
            pts_arr = np.array(pts, dtype=np.int32)

            # 2) 눈 외곽 그리기 (초록 선)
            left_eye = pts_arr[LEFT_EYE_OUTER]
            right_eye = pts_arr[RIGHT_EYE_OUTER]
            cv2.polylines(frame, [left_eye], isClosed=True,
                          thickness=2, color=(0, 255, 0))
            cv2.polylines(frame, [right_eye], isClosed=True,
                          thickness=2, color=(0, 255, 0))

            # 3) 홍채 중심만 찍기 (빨간 점)
            cv2.circle(frame, tuple(pts_arr[468]), radius=4, color=(
                0, 0, 255), thickness=-1)
            cv2.circle(frame, tuple(pts_arr[473]), radius=4, color=(
                0, 0, 255), thickness=-1)

    cv2.imshow('Eye & Iris Contours', frame)
    if cv2.waitKey(1) & 0xFF == 27:  # ESC 키 누르면 종료
        break

cap.release()
cv2.destroyAllWindows()
