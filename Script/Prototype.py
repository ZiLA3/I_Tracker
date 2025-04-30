import cv2 as cv
import mediapipe as mp #Google Open-Source

mp_face_mesh = mp.solutions.face_mesh
face_mesh = mp_face_mesh.FaceMesh(static_image_mode=False,
                                   max_num_faces=1,
                                   refine_landmarks=True,
                                   min_detection_confidence=0.5,
                                   min_tracking_confidence=0.5)

camera_video = cv.VideoCapture(0)

while camera_video.isOpened():
    success, image = camera_video.read()
    if not success:
        break

    # MediaPipe는 RGB 이미지 입력 필요
    image_rgb = cv.cvtColor(image, cv.COLOR_BGR2RGB)
    results = face_mesh.process(image_rgb)

    if results.multi_face_landmarks:
        for face_landmarks in results.multi_face_landmarks:
            # 왼쪽, 오른쪽 홍채 중심 landmark: 468, 473

            h, w, _ = image.shape

            ## 왼쪽 홍채 그리기
            left_iris = face_landmarks.landmark[468]
            clx = int(left_iris.x * w)
            cly = int(left_iris.y * h)
            cv.circle(image, (clx, cly), 3, (0, 255, 0), -1)

            # 오른쪽 홍채 그리기
            right_iris = face_landmarks.landmark[473]
            crx = int(right_iris.x * w)
            cry = int(right_iris.y * h)
            cv.circle(image, (crx, cry), 3, (0, 255, 0), -1)

            # 홍채 중심 그리기
            cx = (clx + crx) // 2
            cy = (cry + cly) // 2
            cv.circle(image, (cx, cy), 3, (0, 0, 255), -1)

    cv.imshow('Iris Tracking', image)
    if cv.waitKey(5) & 0xFF == 27:
        break
camera_video.release()
