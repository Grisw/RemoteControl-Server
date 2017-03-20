#include "stdafx.h"
#include <iostream>

BYTE* lastFrame;

extern "C" __declspec(dllexport) HBITMAP __stdcall capture(int reset)
{
	HDC hDC = ::GetDC(HWND(NULL));//��ȡ��ĻDC
	int x = ::GetDeviceCaps(hDC, HORZRES);
	int y = ::GetDeviceCaps(hDC, VERTRES);
	HDC hDCMem = ::CreateCompatibleDC(hDC);//��������DC  
	HBITMAP hBitMap = ::CreateCompatibleBitmap(hDC, x, y);//��������λͼ
	HBITMAP hOldMap = (HBITMAP)::SelectObject(hDCMem, hBitMap);//��λͼѡ��DC�����淵��ֵ
	BitBlt(hDCMem, 0, 0, x, y, hDC, 0, 0, SRCCOPY);//����ĻDC��ͼ���Ƶ��ڴ�DC��
	POINT po;
	GetCursorPos(&po);
	CURSORINFO ci;
	ci.cbSize = sizeof(ci);
	GetCursorInfo(&ci);
	DrawIcon(hDCMem, po.x - 10, po.y - 10, ci.hCursor);
	hBitMap = (HBITMAP)SelectObject(hDCMem, hOldMap);

	BITMAP bmp;
	GetObject(hBitMap, sizeof(BITMAP), &bmp);
	int nChannels = bmp.bmBitsPixel == 1 ? 1 : bmp.bmBitsPixel / 8;
	int lsize = bmp.bmHeight*bmp.bmWidth*nChannels;
	BYTE* data = new BYTE[lsize];
	GetBitmapBits(hBitMap, lsize, data);
	if (lastFrame != NULL && reset == 0) {
		for (int i = 0;i < lsize;i++) {
			data[i] = data[i] ^ lastFrame[i];
		}
	}
	else {
		delete[] lastFrame;
		lastFrame = new BYTE[lsize]();
	}
	SetBitmapBits(hBitMap, lsize, data);
	for (int i = 0;i < lsize;i++) {
		lastFrame[i] = data[i] ^ lastFrame[i];
	}
	delete[] data;
	return hBitMap;
}

BYTE* lFr;

extern "C" __declspec(dllexport) HBITMAP __stdcall cal(HBITMAP hBitMap)
{
	BITMAP bmp;
	GetObject(hBitMap, sizeof(BITMAP), &bmp);
	int nChannels = bmp.bmBitsPixel == 1 ? 1 : bmp.bmBitsPixel / 8;
	int lsize = bmp.bmHeight*bmp.bmWidth*nChannels;
	BYTE* data = new BYTE[lsize];
	BYTE* output = new BYTE[lsize];
	GetBitmapBits(hBitMap, lsize, data);
	if (lFr != NULL) {
		printf("w:%d, h:%d, ch:%d, size: %d\n", bmp.bmWidth, bmp.bmHeight, nChannels, lsize);
		for (int i = 0;i < lsize;i++) {
			output[i] = data[i] ^ lFr[i];
		}
	}
	else {
		output = data;
	}
	lFr = data;
	SetBitmapBits(hBitMap, lsize, output);
	return hBitMap;
}