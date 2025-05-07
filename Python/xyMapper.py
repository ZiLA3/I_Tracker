import cv2
import numpy as np

class xyMapper:
    def __init__(self, xy_mid, xy_lt, xy_ld, xy_rt, xy_rd, xy_max, xy_max_cam):
        """
        초기화 메서드: 아이트래킹 좌표를 화면 좌표로 매핑하기 위한 파라미터 설정.
        카메라 왜곡을 측정하고 보정하는 기능 추가.
        
        Args:
            xy_mid (tuple): 카메라 좌표계의 중앙점 (x, y)
            xy_lt (tuple): 좌상단 좌표 (x, y)
            xy_ld (tuple): 좌하단 좌표 (x, y)
            xy_rt (tuple): 우상단 좌표 (x, y)
            xy_rd (tuple): 우하단 좌표 (x, y)
            xy_max (tuple): 화면 송출 화면 크기 (width, height)
            xy_max_cam (tuple): 카메라 송출 화면 크기 (width, height)
        """
        self.x_max, self.y_max = xy_max
        self.x_max_cam, self.y_max_cam = xy_max_cam
        self.xy_mid = xy_mid
        
        # 왜곡 계수 저장
        self.distortion_coeffs = None
        
        # 카메라 좌표와 화면 좌표를 매핑하기 위한 변환 행렬 계산
        self._compute_mapping_matrix(xy_mid, xy_lt, xy_ld, xy_rt, xy_rd)

    def _estimate_distortion(self, xy_mid, xy_lt, xy_ld, xy_rt, xy_rd):
        """
        중앙점과 코너 포인트를 이용해 카메라 왜곡 정도를 추정.
        
        Returns:
            np.ndarray: 왜곡 계수 (k1, k2, p1, p2, k3)
        """
        # 이상적인 좌표 (왜곡이 없는 경우의 예상 위치)
        ideal_mid = np.array([self.x_max_cam / 2, self.y_max_cam / 2], dtype=np.float32)
        ideal_corners = np.float32([
            [0, 0],  # lt
            [self.x_max_cam, 0],  # rt
            [0, self.y_max_cam],  # ld
            [self.x_max_cam, self.y_max_cam]  # rd
        ])
        
        # 실제 측정된 좌표
        actual_corners = np.float32([xy_lt, xy_rt, xy_ld, xy_rd])
        actual_mid = np.float32(xy_mid)
        
        # 왜곡에 의한 오차 계산
        mid_offset = actual_mid - ideal_mid
        corner_offsets = actual_corners - ideal_corners
        
        # 간단한 왜곡 모델: 방사 왜곡(radial)과 접선 왜곡(tangential) 계수 추정
        # k1, k2: 방사 왜곡, p1, p2: 접선 왜곡, k3: 추가 방사 왜곡
        dist_coeffs = np.zeros((5, 1), dtype=np.float32)
        
        # 중앙점 오차를 이용해 초기 방사 왜곡 추정
        r_mid = np.sqrt(mid_offset[0]**2 + mid_offset[1]**2)
        if r_mid > 0:
            dist_coeffs[0] = np.mean(np.linalg.norm(corner_offsets, axis=1)) / r_mid  # k1
        
        # 코너 오차를 이용해 접선 왜곡 추정
        dist_coeffs[2] = np.mean(corner_offsets[:, 0]) / self.x_max_cam  # p1
        dist_coeffs[3] = np.mean(corner_offsets[:, 1]) / self.y_max_cam  # p2
        
        return dist_coeffs

    def _compute_mapping_matrix(self, xy_mid, xy_lt, xy_ld, xy_rt, xy_rd):
        """
        카메라 좌표와 화면 좌표 간의 변환 행렬을 계산.
        왜곡 보정을 적용한 원근 변환 행렬 생성.
        """
        # 왜곡 계수 추정
        self.distortion_coeffs = self._estimate_distortion(xy_mid, xy_lt, xy_ld, xy_rt, xy_rd)
        
        # 왜곡 보정된 좌표 계산
        src_points = np.float32([xy_lt, xy_rt, xy_ld, xy_rd])
        
        # 카메라 행렬 (단순 근사)
        camera_matrix = np.float32([
            [self.x_max_cam, 0, self.x_max_cam / 2],
            [0, self.y_max_cam, self.y_max_cam / 2],
            [0, 0, 1]
        ])
        
        # 왜곡 보정 적용
        undistorted_points = cv2.undistortPoints(
            src_points.reshape(-1, 1, 2),
            camera_matrix,
            self.distortion_coeffs,
            None,
            camera_matrix
        )
        undistorted_points = undistorted_points.reshape(-1, 2)
        
        # 화면 좌표 (출력)
        dst_points = np.float32([[0, 0], [self.x_max, 0], [0, self.y_max], [self.x_max, self.y_max]])
        
        # 원근 변환 행렬 계산
        self.transform_matrix = cv2.getPerspectiveTransform(undistorted_points, dst_points)

    def map_coordinates(self, xy):
        """
        주어진 카메라 좌표를 화면 좌표로 매핑.
        왜곡 보정을 적용.
        
        Args:
            xy (tuple): 매핑할 카메라 좌표 (x, y)
        
        Returns:
            tuple: 매핑된 화면 좌표 (x, y)
        """
        # 카메라 행렬
        camera_matrix = np.float32([
            [self.x_max_cam, 0, self.x_max_cam / 2],
            [0, self.y_max_cam, self.y_max_cam / 2],
            [0, 0, 1]
        ])
        
        # 왜곡 보정
        point = np.array([[[xy[0], xy[1]]]], dtype=np.float32)
        undistorted_point = cv2.undistortPoints(
            point,
            camera_matrix,
            self.distortion_coeffs,
            None,
            camera_matrix
        )
        
        # 원근 변환 적용
        mapped_point = cv2.perspectiveTransform(undistorted_point, self.transform_matrix)
        
        # 결과 좌표 추출 및 화면 크기 내로 제한
        x = np.clip(mapped_point[0, 0, 0], 0, self.x_max)
        y = np.clip(mapped_point[0, 0, 1], 0, self.y_max)
        
        return int(x), int(y)

    def map_coordinates_batch(self, xy_list):
        """
        여러 카메라 좌표를 한 번에 화면 좌표로 매핑 (실시간 처리 최적화).
        왜곡 보정을 적용.
        
        Args:
            xy_list (list): 매핑할 카메라 좌표 리스트 [(x1, y1), (x2, y2), ...]
        
        Returns:
            list: 매핑된 화면 좌표 리스트 [(x1, y1), (x2, y2), ...]
        """
        # 카메라 행렬
        camera_matrix = np.float32([
            [self.x_max_cam, 0, self.x_max_cam / 2],
            [0, self.y_max_cam, self.y_max_cam / 2],
            [0, 0, 1]
        ])
        
        # 입력 좌표
        points = np.array([[[x, y]] for x, y in xy_list], dtype=np.float32)
        
        # 왜곡 보정
        undistorted_points = cv2.undistortPoints(
            points,
            camera_matrix,
            self.distortion_coeffs,
            None,
            camera_matrix
        )
        
        # 원근 변환 적용
        mapped_points = cv2.perspectiveTransform(undistorted_points, self.transform_matrix)
        
        # 결과 좌표를 화면 크기 내로 제한
        mapped_points[:, 0, 0] = np.clip(mapped_points[:, 0, 0], 0, self.x_max)
        mapped_points[:, 0, 1] = np.clip(mapped_points[:, 0, 1], 0, self.y_max)
        
        return [(int(x), int(y)) for [[x, y]] in mapped_points]