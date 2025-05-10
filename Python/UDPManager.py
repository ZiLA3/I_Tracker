import socket
import json
import time
import asyncio

class UDPManager:
    """
    유니티와 통신하는 UDPManager 클래스
    """
    def __init__(self, ip="127.0.0.1", send_port=5000, receive_port=5001):
        """
        args는 제곧내
        """
        self.ip = ip
        self.send_port = send_port
        self.receive_port = receive_port
        
        # 송신용 소켓
        self.send_sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
        
        # 수신용 소켓 (비동기 이벤트 루프에서 사용할 것임)
        self.receive_sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
        self.receive_sock.bind((self.ip, self.receive_port))
        self.receive_sock.setblocking(False)  # 비동기 처리를 위한 논블로킹 모드 설정
        
        # 이벤트 콜백 집합합
        self.callbacks = {}
        
    def send(self, data):
        """데이터를 유니티로 전송"""
        message = json.dumps(data).encode('utf-8')
        self.send_sock.sendto(message, (self.ip, self.send_port))

    def receive(self, message_type=None):
        """
        초기화를 위한 초기 동기 방식의 메시지 수신
        절대로 비동기 수신중 사용하지 마시오!!!!!!
        """
        self.receive_sock.setblocking(True)
        while True:
            try:
                # 비동기적으로 데이터 수신
                data, addr = self.receive_sock.recvfrom(1024)
                
                try:
                    message = json.loads(data.decode('utf-8'))
                    if message_type is None:
                        message_type = message.get('type')
                    
                    # 등록된 콜백 실행
                    if message_type in self.callbacks:
                        callback_result = self.callbacks[message_type](message)
                        # 콜백에서 False를 반환하면 대기시간 후후 다음 루프 재시작
                        if callback_result is False:
                            time.sleep(0.1)
                            continue
                        # 콜백 정상 작동 후 수신 종료 (동기화)
                        else:
                            break
                except json.JSONDecodeError:
                    print(f"잘못된 JSON 형식: {data.decode('utf-8')}")
                except Exception as e:
                    print(f"메시지 처리 중 오류: {e}")
            
            except ConnectionResetError:
                print("연결이 재설정되었습니다.")
            except Exception as e:
                if not isinstance(e, asyncio.CancelledError):
                    print(f"수신 중 오류: {e}")
                    # 짧은 대기 후 다시 시도
                    time.sleep(0.1)

        self.receive_sock.setblocking(False)
        
    
    def register_callback(self, message_type, callback):
        """특정 메시지 타입에 대한 콜백 함수 등록"""
        self.callbacks[message_type] = callback
    
    async def start_listening(self):
        """비동기 방식으로 메시지 수신 루프 시작"""
        loop = asyncio.get_running_loop()
        
        while True:
            try:
                # 비동기적으로 데이터 수신
                data, addr = await loop.sock_recvfrom(self.receive_sock, 1024)
                
                try:
                    message = json.loads(data.decode('utf-8'))
                    message_type = message.get('type')
                    
                    # 등록된 콜백 실행
                    if message_type in self.callbacks:
                        callback_result = self.callbacks[message_type](message)
                        # 콜백에서 False를 반환하면 대기 시간 후 다음 루프 재시작
                        if callback_result is False:
                            await asyncio.sleep(0.1)
                            continue
                except json.JSONDecodeError:
                    print(f"잘못된 JSON 형식: {data.decode('utf-8')}")
                except Exception as e:
                    print(f"메시지 처리 중 오류: {e}")
            
            except ConnectionResetError:
                print("연결이 재설정되었습니다.")
            except Exception as e:
                if not isinstance(e, asyncio.CancelledError):
                    print(f"수신 중 오류: {e}")
                    # 짧은 대기 후 다시 시도
                    await asyncio.sleep(0.1)
    
    def close(self):
        """소켓 연결 종료"""
        self.send_sock.close()
        self.receive_sock.close()
