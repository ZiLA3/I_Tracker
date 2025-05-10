import mediapipe as mp
import cv2 as cv

class irisTracker:
    def __init__(self, camera=cv.VideoCapture(0)):
        self.mp_face_mesh = mp.solutions.face_mesh
        self.face_mesh = self.mp_face_mesh.FaceMesh(
            static_image_mode=False,
            max_num_faces=1,
            refine_landmarks=True,
            min_detection_confidence=0.5,
            min_tracking_confidence=0.5
        )
        self.camera = camera
    
    def get_iris_position(self):
        """현재 홍채 위치를 반환"""
        success, image = self.camera.read()
        image = cv.flip(image, 1) # 카메라 좌우반전
        if not success:
            return None, None
        
        # MediaPipe는 RGB 이미지 입력 필요
        image_rgb = cv.cvtColor(image, cv.COLOR_BGR2RGB)
        results = self.face_mesh.process(image_rgb)
        
        iris_position = None
        
        if results.multi_face_landmarks:
            for face_landmarks in results.multi_face_landmarks:
                h, w, _ = image.shape
                
                # 왼쪽, 오른쪽 홍채 중심 landmark: 468, 473
                left_iris = face_landmarks.landmark[468]
                clx = int(left_iris.x * w)
                cly = int(left_iris.y * h)
                
                right_iris = face_landmarks.landmark[473]
                crx = int(right_iris.x * w)
                cry = int(right_iris.y * h)
                
                # 홍채 중심 계산
                cx = (clx + crx) // 2
                cy = (cly + cry) // 2
                
                # 화면에 홍채 위치 표시 (디버깅용)
                cv.circle(image, (clx, cly), 3, (0, 255, 0), -1)  # 왼쪽 홍채
                cv.circle(image, (crx, cry), 3, (0, 255, 0), -1)  # 오른쪽 홍채
                cv.circle(image, (cx, cy), 3, (0, 0, 255), -1)    # 홍채 중심
                
                iris_position = (cx, cy)
        
        return iris_position, image
