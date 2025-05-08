import cv2
import numpy as np

class xyMapper:
    def __init__(self, xy_lt, xy_ld, xy_rt, xy_rd, xy_Screen):
        """
        초기화 메서드: 아이트래킹 좌표를 화면 좌표로 매핑하기 위한 파라미터 설정.
        
        Args:
            xy_lt (tuple): 좌상단 홍채 좌표 (x, y)
            xy_ld (tuple): 좌하단 홍채 좌표 (x, y)
            xy_rt (tuple): 우상단 홍채 좌표 (x, y)
            xy_rd (tuple): 우하단 홍채 좌표 (x, y)
        """

        # 모니터 화면 크기
        self.xy_Screen = xy_Screen

        calibration_points = {
            (xy_lt[0], xy_lt[1]): (0, 0), # 좌상
            (xy_rt[0], xy_rt[1]): (xy_Screen[0], 0), # 우상
            (xy_ld[0], xy_ld[1]): (0, xy_Screen[1]), # 좌하
            (xy_rd[0], xy_rd[1]): (xy_Screen[0], xy_Screen[1]) # 우하
        }
        
        # 현재 홍채의 좌표를 매핑하기 위한 호모그래피 행렬
        self.H = None
        self.compute_homography(calibration_points)

    # 호모그래피 행렬 계산
    def compute_homography(self, calibration_points):
        src_points = np.array([list(k) for k in calibration_points.keys()], dtype=np.float32)
        dst_points = np.array([list(v) for v in calibration_points.values()], dtype=np.float32)
        self.H, _ = cv2.findHomography(src_points, dst_points, cv2.RANSAC)

    def map_coordinates(self, xy_iris):
        """
        주어진 홍채의 좌표를 호모그래피 행렬을 바탕으로 화면 좌표로 매핑.
        
        Args:
            xy_iris (tuple): 매핑할 카메라 좌표 (x, y)
        
        Returns:
            tuple: 매핑된 화면 좌표 (x, y)
        """

        point = np.array([[xy_iris[0], xy_iris[1]]], dtype=np.float32)
        transformed = cv2.perspectiveTransform(point[None, :, :], self.H)
        screen_x, screen_y= transformed[0, 0]

        # 결과 좌표 추출 및 화면 크기 내로 제한
        x = np.clip(screen_x, 0, self.xy_Screen[0])
        y = np.clip(screen_y, 0, self.xy_Screen[1])
        
        return (x,y)
    