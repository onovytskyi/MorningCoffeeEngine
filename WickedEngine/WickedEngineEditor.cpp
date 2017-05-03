#include "stdafx.h"
#include "WickedEngineEditor.h"
#include "Editor.h"

#include <fstream>

#define MAX_LOADSTRING 100

extern "C" {
	_declspec(dllexport) DWORD NvOptimusEnablement = 0x00000001;
}

// Global Variables:
HINSTANCE hInst;                                // current instance
WCHAR szTitle[MAX_LOADSTRING];                  // The title bar text
WCHAR szWindowClass[MAX_LOADSTRING];            // the main window class name
Editor editor;


enum Hotkeys
{
	UNUSED = 0,
	PRINTSCREEN = 1,
};

// Forward declarations of functions included in this code module:
ATOM                MyRegisterClass(HINSTANCE hInstance);
BOOL                InitInstance(HINSTANCE, int);
LRESULT CALLBACK    WndProc(HWND, UINT, WPARAM, LPARAM);
INT_PTR CALLBACK    About(HWND, UINT, WPARAM, LPARAM);

int APIENTRY wWinMain(_In_ HINSTANCE hInstance,
                     _In_opt_ HINSTANCE hPrevInstance,
                     _In_ LPWSTR    lpCmdLine,
                     _In_ int       nCmdShow)
{
    UNREFERENCED_PARAMETER(hPrevInstance);
    UNREFERENCED_PARAMETER(lpCmdLine);

    // TODO: Place code here.

    // Initialize global strings
    LoadStringW(hInstance, IDS_APP_TITLE, szTitle, MAX_LOADSTRING);
    LoadStringW(hInstance, IDC_WICKEDENGINEGAME, szWindowClass, MAX_LOADSTRING);
    MyRegisterClass(hInstance);

    // Perform application initialization:
    if (!InitInstance (hInstance, nCmdShow))
    {
        return FALSE;
    }

    HACCEL hAccelTable = LoadAccelerators(hInstance, MAKEINTRESOURCE(IDC_WICKEDENGINEGAME));

	editor.Initialize();


	MSG msg = { 0 };
	while (msg.message != WM_QUIT)
	{
		if (PeekMessage(&msg, NULL, 0, 0, PM_REMOVE)) {
			TranslateMessage(&msg);
			DispatchMessage(&msg);
		}
		else {

			editor.run();

		}
	}

    return (int) msg.wParam;
}



//
//  FUNCTION: MyRegisterClass()
//
//  PURPOSE: Registers the window class.
//
ATOM MyRegisterClass(HINSTANCE hInstance)
{
    WNDCLASSEXW wcex;

    wcex.cbSize = sizeof(WNDCLASSEX);

    wcex.style          = CS_HREDRAW | CS_VREDRAW;
    wcex.lpfnWndProc    = WndProc;
    wcex.cbClsExtra     = 0;
    wcex.cbWndExtra     = 0;
    wcex.hInstance      = hInstance;
    wcex.hIcon          = LoadIcon(hInstance, MAKEINTRESOURCE(IDI_WICKEDENGINEGAME));
    wcex.hCursor        = LoadCursor(nullptr, IDC_ARROW);
    wcex.hbrBackground  = (HBRUSH)(COLOR_WINDOW+1);
    //wcex.lpszMenuName   = MAKEINTRESOURCEW(IDC_WICKEDENGINEGAME);
	wcex.lpszMenuName = L"";
    wcex.lpszClassName  = szWindowClass;
    wcex.hIconSm        = LoadIcon(wcex.hInstance, MAKEINTRESOURCE(IDI_SMALL));

    return RegisterClassExW(&wcex);
}

//
//   FUNCTION: InitInstance(HINSTANCE, int)
//
//   PURPOSE: Saves instance handle and creates main window
//
//   COMMENTS:
//
//        In this function, we save the instance handle in a global variable and
//        create and display the main program window.
//
BOOL InitInstance(HINSTANCE hInstance, int nCmdShow)
{
   hInst = hInstance; // Store instance handle in our global variable

   int x = CW_USEDEFAULT, y = 0, w = CW_USEDEFAULT, h = 0;
   bool borderless = false;
   string voidStr = "";

   ifstream file("config.ini");
   if (file.is_open())
   {
	   int enabled;
	   file >> voidStr >> enabled;
	   if (enabled != 0)
	   {
		   file >> voidStr >> x >> voidStr >> y >> voidStr >> w >> voidStr >> h >> voidStr >> editor.fullscreen >> voidStr >> borderless;
		   editor.screenW = w;
		   editor.screenH = h;
	   }
   }
   file.close();

   HWND hWnd = NULL;

   if (borderless)
   {
	   hWnd = CreateWindowEx(WS_EX_APPWINDOW,
		     szWindowClass,
		     szTitle,
		     WS_POPUP,
		     x, y, w, h,
		     NULL,
		     NULL,
		     hInstance,
		     NULL
		    );
   }
   else
   {
	   hWnd = CreateWindow(szWindowClass, szTitle, WS_OVERLAPPEDWINDOW,
		   x, y, w, h, NULL, NULL, hInstance, NULL);
   }

   if (!hWnd)
   {
      return FALSE;
   }

   if (!editor.setWindow(hWnd, hInst))
	   return false;


   ShowWindow(hWnd, nCmdShow);
   UpdateWindow(hWnd);

   RegisterHotKey(hWnd, PRINTSCREEN, 0, VK_SNAPSHOT);

   return TRUE;
}

//
//  FUNCTION: WndProc(HWND, UINT, WPARAM, LPARAM)
//
//  PURPOSE:  Processes messages for the main window.
//
//  WM_COMMAND  - process the application menu
//  WM_PAINT    - Paint the main window
//  WM_DESTROY  - post a quit message and return
//
//
LRESULT CALLBACK WndProc(HWND hWnd, UINT message, WPARAM wParam, LPARAM lParam)
{
    switch (message)
    {
    case WM_COMMAND:
        {
            int wmId = LOWORD(wParam);
            // Parse the menu selections:
            switch (wmId)
            {
            case IDM_ABOUT:
                DialogBox(hInst, MAKEINTRESOURCE(IDD_ABOUTBOX), hWnd, About);
                break;
            case IDM_EXIT:
                DestroyWindow(hWnd);
                break;
            default:
                return DefWindowProc(hWnd, message, wParam, lParam);
            }
        } 
        break;
	case WM_MBUTTONDOWN:
		ShowCursor(false);
		break;
	case WM_MBUTTONUP:
		ShowCursor(true);
		break;
	case WM_MOUSEWHEEL:
		{
		XMFLOAT4 pointer = wiInputManager::GetInstance()->getpointer();
		float delta = GET_WHEEL_DELTA_WPARAM(wParam) / (float)WHEEL_DELTA;
		wiInputManager::GetInstance()->setpointer(XMFLOAT4(pointer.x, pointer.y, delta, 0));
		}
		break;
	case WM_KEYDOWN:
		switch (wParam)
		{
		case VK_HOME:
			wiBackLog::Toggle();
			break;
		case VK_UP:
			if (wiBackLog::isActive())
				wiBackLog::historyPrev();
			break;
		case VK_DOWN:
			if (wiBackLog::isActive())
				wiBackLog::historyNext();
			break;
		case VK_NEXT:
			if (wiBackLog::isActive())
				wiBackLog::Scroll(10);
			break;
		case VK_PRIOR:
			if (wiBackLog::isActive())
				wiBackLog::Scroll(-10);
			break;
		default:
			break;
		}
		break;
	case WM_HOTKEY:
		switch (wParam)
		{
		case PRINTSCREEN:
			{
				wiHelper::screenshot();
			}
			break;
		default:
			break;
		}
		break;
	case WM_CHAR:
		switch (wParam)
		{
		case VK_BACK:
			if (wiBackLog::isActive())
				wiBackLog::deletefromInput();
			break;
		case VK_RETURN:
			if (wiBackLog::isActive())
				wiBackLog::acceptInput();
			break;
		default:
			if (wiBackLog::isActive()) {
				const char c = (const char)(TCHAR)wParam;
				wiBackLog::input(c);
			}
			break;
		}
		break;
    case WM_PAINT:
        {
            PAINTSTRUCT ps;
            HDC hdc = BeginPaint(hWnd, &ps);
            // TODO: Add any drawing code that uses hdc here...
            EndPaint(hWnd, &ps);
        }
        break;
    case WM_DESTROY:
        PostQuitMessage(0);
        break;
    default:
        return DefWindowProc(hWnd, message, wParam, lParam);
    }
    return 0;
}

// Message handler for about box.
INT_PTR CALLBACK About(HWND hDlg, UINT message, WPARAM wParam, LPARAM lParam)
{
    UNREFERENCED_PARAMETER(lParam);
    switch (message)
    {
    case WM_INITDIALOG:
        return (INT_PTR)TRUE;

    case WM_COMMAND:
        if (LOWORD(wParam) == IDOK || LOWORD(wParam) == IDCANCEL)
        {
            EndDialog(hDlg, LOWORD(wParam));
            return (INT_PTR)TRUE;
        }
        break;
    }
    return (INT_PTR)FALSE;
}
