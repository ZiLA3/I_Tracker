# -*- mode: python ; coding: utf-8 -*-


a = Analysis(
    ['Main.py'],
    pathex=[],
    binaries=[],
    datas=[
        # 얼굴 인식 관련 모델
        ('/home/zila3/miniconda3/lib/python3.12/site-packages/mediapipe/modules/face_landmark', 'mediapipe/modules/face_landmark'),
        # 손 인식 관련 모델
        ('/home/zila3/miniconda3/lib/python3.12/site-packages/mediapipe/modules/hand_landmark', 'mediapipe/modules/hand_landmark'),
        # 얼굴 탐지 모델
        ('/home/zila3/miniconda3/lib/python3.12/site-packages/mediapipe/modules/face_detection', 'mediapipe/modules/face_detection'),
        # 손바닥 탐지 모델
        ('/home/zila3/miniconda3/lib/python3.12/site-packages/mediapipe/modules/palm_detection', 'mediapipe/modules/palm_detection'),
    ],
    hiddenimports=[],
    hookspath=[],
    hooksconfig={},
    runtime_hooks=[],
    excludes=[],
    noarchive=False,
    optimize=0,
)
pyz = PYZ(a.pure)

exe = EXE(
    pyz,
    a.scripts,
    a.binaries,
    a.datas,
    [],
    name='Tracker',
    debug=False,
    bootloader_ignore_signals=False,
    strip=False,
    upx=True,
    upx_exclude=[],
    runtime_tmpdir=None,
    console=True,
    disable_windowed_traceback=False,
    argv_emulation=False,
    target_arch=None,
    codesign_identity=None,
    entitlements_file=None,
)

coll = COLLECT(
    exe,
    a.binaries,
    a.zipfiles,
    a.datas,
    strip=False,
    upx=True,
    upx_exclude=[],
    name='Tracker'
)
