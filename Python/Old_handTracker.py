import cv2
from cvzone.HandTrackingModule import HandDetector
import mediapipe as mp
import socket

# TODO : hands나 eye가 없을 때 예외처리 추가하기

# Parameters
width, height = 1920, 1080

# WebCam
cap = cv2.VideoCapture(0)
cap.set(3, width)
cap.set(4, height)

# Detectors
handDetector = HandDetector(maxHands=1, detectionCon=0.8)
# MediaPipe FaceMesh 초기화
mp_face_mesh = mp.solutions.face_mesh
face_mesh = mp_face_mesh.FaceMesh(static_image_mode=False,
                                  max_num_faces=1,
                                  refine_landmarks=True,
                                  min_detection_confidence=0.5,
                                  min_tracking_confidence=0.5)

# UDP sockets
sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
handAddressPort = ("127.0.0.1", 5052)

# Iris landmarks (왼쪽, 오른쪽)
iris_indices = [468, 473]

while True:
    success, img = cap.read()
    if not success:
        break

    # 1) Hand Detection
    hands, img = handDetector.findHands(img, flipType = True) # flipType = True: 좌우 반전
    img_rgb = cv2.cvtColor(img, cv2.COLOR_BGR2RGB)
    data = []

    if hands:
        lmList = hands[0]['lmList']        
        for x, y, z in lmList:
            data.extend([x, height - y, z])
            
     # MediaPipe는 RGB 이미지 입력 필요
    img_rgb = cv2.cvtColor(img, cv2.COLOR_BGR2RGB)
    results = face_mesh.process(img_rgb)

    if results.multi_face_landmarks:
        for face_landmarks in results.multi_face_landmarks:
            # 왼쪽, 오른쪽 홍채 중심 landmark: 468, 473

            h, w, _ = img.shape

            # 왼쪽 홍채 그리기
            left_iris = face_landmarks.landmark[468]
            clx = int(left_iris.x * w)
            cly = int(left_iris.y * h)
            cv2.circle(img, (clx, cly), 3, (0, 255, 0), -1)

            # 오른쪽 홍채 그리기
            right_iris = face_landmarks.landmark[473]
            crx = int(right_iris.x * w)
            cry = int(right_iris.y * h)
            cv2.circle(img, (crx, cry), 3, (0, 255, 0), -1)

            # 홍채 중심 그리기
            cx = (clx + crx) // 2
            cy = (cry + cly) // 2
            cv2.circle(img, (cx, cy), 3, (0, 0, 255), -1)
            # ★ eye_data 생성 및 UDP 전송 추가 부분 ★
            data.extend([clx, h - cly, crx, h - cry])
    
    sock.sendto(str.encode(str(data)), handAddressPort)

    cv2.imshow("img", img)

    if cv2.waitKey(1) & 0xFF == 27:
        break

cap.release()
cv2.destroyAllWindows()
