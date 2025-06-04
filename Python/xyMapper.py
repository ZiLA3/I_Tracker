import cv2
import numpy as np

class xyMapper:
    """
    홍채 좌표를 화면 좌표로 매핑하는 클래스
    
    캘리브레이션으로 수집된 4개의 참조점(좌상, 우상, 좌하, 우하)을 바탕으로
    홍채 위치를 화면 좌표로 변환하는 호모그래피 변환을 수행합니다.
    """
    def __init__(self, calibration_points, xy_Screen):
        """
        초기화 메서드: 아이트래킹 좌표를 화면 좌표로 매핑하기 위한 파라미터 설정
        
        Args:
            calibration_points (dict): 4개의 화면 모서리 2차원 좌표 모음 (x,y)
            xy_Screen (tuple): 화면 크기 (width, height)
        """

        xy_lt, xy_rt, xy_ld, xy_rd = (p for p in calibration_points.values())

        # 모니터 화면 크기 저장
        self.xy_Screen = xy_Screen

        # 캘리브레이션 포인트 매핑 설정
        # 키: 카메라 공간 좌표 (홍채 위치), 값: 화면 공간 좌표
        homography_points = {
            (xy_lt[0], xy_lt[1]): (0, xy_Screen[1]),               # 좌상단 모서리
            (xy_rt[0], xy_rt[1]): (xy_Screen[0], xy_Screen[1]),    # 우상단 모서리
            (xy_ld[0], xy_ld[1]): (0, 0),    # 좌하단 모서리
            (xy_rd[0], xy_rd[1]): (xy_Screen[0], 0) # 우하단 모서리
        }
        
        # 호모그래피 행렬 초기화
        self.H = None
        self.compute_homography(homography_points)  # 호모그래피 행렬 계산

    def compute_homography(self, calibration_points):
        """
        호모그래피 행렬 계산
        
        캘리브레이션 포인트를 사용하여 카메라 공간에서 화면 공간으로의 변환 행렬 계산
        
        Args:
            calibration_points (dict): 캘리브레이션 포인트 매핑 (카메라 좌표 -> 화면 좌표)
        """
        # 캘리브레이션 포인트에서 소스 포인트(카메라 좌표) 추출
        src_points = np.array([list(k) for k in calibration_points.keys()], dtype=np.float32)
        # 캘리브레이션 포인트에서 타겟 포인트(화면 좌표) 추출
        dst_points = np.array([list(v) for v in calibration_points.values()], dtype=np.float32)
        
        # RANSAC 알고리즘을 사용하여 호모그래피 행렬 계산
        # RANSAC은 이상치(outlier)에 강인한 추정 방법
        self.H, _ = cv2.findHomography(src_points, dst_points, cv2.RANSAC)

    def map_coordinates(self, xy_iris):
        """
        주어진 홍채의 좌표를 호모그래피 행렬을 바탕으로 화면 좌표로 매핑
        
        Args:
            xy_iris (tuple): 매핑할 카메라 좌표 (x, y)
        
        Returns:
            tuple: 매핑된 화면 좌표 (x, y), 화면 범위 내로 클리핑됨
            
        Process:
            1. 카메라 좌표를 적절한 형식으로 변환
            2. 호모그래피 행렬을 사용하여 투시 변환 적용
            3. 결과 좌표를 화면 크기 내로 제한(클리핑)
        """
        # 입력 좌표를 변환에 적합한 형식으로 준비 (3차원 배열 구조로)
        point = np.array([[xy_iris[0], xy_iris[1]]], dtype=np.float32)
        
        # 호모그래피 행렬을 사용하여 좌표 변환
        transformed = cv2.perspectiveTransform(point[None, :, :], self.H)
        
        # 변환된 결과에서 좌표 추출
        screen_x, screen_y = transformed[0, 0]

        # 결과 좌표를 화면 크기 내로 제한 (0 ~ 화면 크기)
        x = np.clip(screen_x, 0, self.xy_Screen[0])  # x 좌표 클리핑
        y = np.clip(screen_y, 0, self.xy_Screen[1])  # y 좌표 클리핑

        return (x, y)  # 최종 매핑된 화면 좌표 반환