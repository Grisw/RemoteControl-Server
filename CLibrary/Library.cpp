#include "stdafx.h"
#include <iostream>

BYTE* lastFrame;
double lastResolution;

extern "C" __declspec(dllexport) HBITMAP __stdcall capture(double resolution)
{
	HDC hDC = ::GetDC(HWND(NULL));//获取屏幕DC
	int x = ::GetDeviceCaps(hDC, HORZRES);
	int y = ::GetDeviceCaps(hDC, VERTRES);
	HDC hDCMem = ::CreateCompatibleDC(hDC);//创建兼容DC  
	HBITMAP hBitMap = ::CreateCompatibleBitmap(hDC, x*resolution, y*resolution);//创建兼容位图
	HBITMAP hOldMap = (HBITMAP)::SelectObject(hDCMem, hBitMap);//将位图选入DC并保存返回值
	SetStretchBltMode(hDCMem, HALFTONE);
	StretchBlt(hDCMem, 0, 0, x*resolution, y*resolution, hDC, 0, 0, x, y, SRCCOPY);
	POINT po;
	GetCursorPos(&po);
	CURSORINFO ci;
	ci.cbSize = sizeof(ci);
	GetCursorInfo(&ci);
	DrawIcon(hDCMem, (po.x-10)*resolution, (po.y-10)*resolution, ci.hCursor);
	hBitMap = (HBITMAP)SelectObject(hDCMem, hOldMap);
	ReleaseDC(NULL, hDC);
	DeleteDC(hDCMem);

	BITMAP bmp;
	GetObject(hBitMap, sizeof(BITMAP), &bmp);
	int nChannels = bmp.bmBitsPixel == 1 ? 1 : bmp.bmBitsPixel / 8;
	int lsize = bmp.bmHeight*bmp.bmWidth*nChannels;
	BYTE* data = new BYTE[lsize];
	GetBitmapBits(hBitMap, lsize, data);
	if (lastFrame != NULL && lastResolution == resolution) {
		for (int i = 0;i < lsize;i++) {
			data[i] = data[i] ^ lastFrame[i];
		}
	}
	else {
		delete[] lastFrame;
		lastFrame = new BYTE[lsize]();
		lastResolution = resolution;
	}
	SetBitmapBits(hBitMap, lsize, data);
	for (int i = 0;i < lsize;i++) {
		lastFrame[i] = data[i] ^ lastFrame[i];
	}
	delete[] data;
	return hBitMap;
}