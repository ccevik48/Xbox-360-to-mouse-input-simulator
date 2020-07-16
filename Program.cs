using System;
using System.Drawing;
using SharpDX.XInput;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrandonPotter.XBox;
using System.Timers;
using System.Runtime.InteropServices;
using DocumentFormat.OpenXml;
using WindowsInput;
using WindowsInput.Native;

namespace xboxtest
{
    class Program
    {
        static int _x;
        static int _y;

        static void Main(string[] args)
        {

            XBoxControllerWatcher xbcw = new XBoxControllerWatcher();
            xbcw.ControllerConnected += OnControllerConnected;
            xbcw.ControllerDisconnected += OnControllerDisconnected;

            Point defPnt = new Point();
            GetCursorPos(ref defPnt);

            InputSimulator input = new InputSimulator();

            _x = defPnt.X;
            _y = defPnt.Y;

            int clickBuffer = 0;
            int rightClickBuffer = 0;
            int pageNavBuffer = 0;

            while (!Console.KeyAvailable)
            {
                System.Threading.Thread.Sleep(10);
                foreach (var c in XBoxController.GetConnectedControllers())
                {
                    //handle cursor position
                    _x += trueSpeed(c.ThumbLeftX);
                    _y -= trueSpeed(c.ThumbLeftY);
                    SetCursorPos(_x, _y);

                    

                    //handle single left clicks
                    if (c.ButtonAPressed && clickBuffer == 0)
                    {
                        clickBuffer += 1;
                        //DoMouseClickLeft();
                        input.Mouse.LeftButtonClick();
                    }
                    else if ( clickBuffer != 0 && clickBuffer < 12)
                    {
                        clickBuffer += 1;
                    }
                    else //if (clickBuffer > 12)
                    {
                        clickBuffer = 0;
                    }
                    //hold left click
                    if (c.ButtonShoulderLeftPressed)
                    {
                        input.Mouse.LeftButtonDown();
                    }
                    

                    //handle right clicks
                    if (c.ButtonBPressed && rightClickBuffer == 0)
                    {
                        rightClickBuffer += 1;
                        //DoMouseClickRight();
                        input.Mouse.RightButtonClick();
                    }
                    else if (rightClickBuffer != 0 && rightClickBuffer < 21)
                    {
                        rightClickBuffer += 1;
                    }
                    else //if (clickBuffer > 21)
                    {
                        rightClickBuffer = 0;
                    }


                    //handle page scrolling
                    if(trueSpeed(c.ThumbRightY) > 2)
                    {
                        MouseWheelUp();
                    }
                    if (trueSpeed(c.ThumbRightY) < -2)
                    {
                        MouseWheelDown();
                    }
                    if (trueSpeed(c.ThumbRightX) > 2)
                    {
                        MouseWheelRight();
                    }
                    if (trueSpeed(c.ThumbRightX) < -2)
                    {
                        MouseWheelLeft();
                    }


                    //handle page navigation
                    if (c.ButtonLeftPressed && pageNavBuffer == 0)
                    {
                        pageNavBuffer += 1;
                        input.Keyboard.KeyPress(VirtualKeyCode.BROWSER_BACK);
                    }
                    else if (pageNavBuffer != 0 && pageNavBuffer < 12)
                    {
                        pageNavBuffer += 1;
                    }
                    else //if (clickBuffer > 12)
                    {
                        pageNavBuffer = 0;
                    }
                    if (c.ButtonRightPressed && pageNavBuffer == 0)
                    {
                        pageNavBuffer += 1;
                        input.Keyboard.KeyPress(VirtualKeyCode.BROWSER_FORWARD);
                    }
                    else if (pageNavBuffer != 0 && pageNavBuffer < 12)
                    {
                        pageNavBuffer += 1;
                    }
                    else //if (clickBuffer > 12)
                    {
                        pageNavBuffer = 0;
                    }




                }
                
            }

        }

        //normal joystick range is 1 to 100, need -50 to 49 for easier calculation
        public static int trueSpeed(double x)
        {
            x -= 50;
            x /= 6;

            return (int) x;
        }





        private static void OnControllerDisconnected(XBoxController controller)
        {
            Console.WriteLine("Controller Disconnected: Player " + controller.PlayerIndex.ToString());
        }

        private static void OnControllerConnected(XBoxController controller)
        {
            Console.WriteLine("Controller Connected: Player " + controller.PlayerIndex.ToString());
        }




        //implement DllImport functions here
        [DllImport("user32.dll")]
        static extern bool SetCursorPos(int X, int Y);
        
        [DllImport("user32.dll")]
        static extern bool GetCursorPos(ref Point lpPoint);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void mouse_event(long dwFlags, long dx, long dy, long cButtons, long dwExtraInfo);

        private const int MOUSEEVENTF_LEFTDOWN = 0x02;
        private const int MOUSEEVENTF_LEFTUP = 0x04;
        private const int MOUSEEVENTF_RIGHTDOWN = 0x08;
        private const int MOUSEEVENTF_RIGHTUP = 0x10;
        private const int MOUSEEVENTF_WHEEL = 0x0800;
        private const int MOUSEEVENTF_HWHEEL = 0x01000;

        public static void MouseWheelUp()
        {
            mouse_event(MOUSEEVENTF_WHEEL, 0, 0, 40, 0);
        }
        public static void MouseWheelDown()
        {
            mouse_event(MOUSEEVENTF_WHEEL, 0, 0, -40, 0);
        }


        public static void MouseWheelLeft()
        {
            mouse_event(MOUSEEVENTF_HWHEEL, 0, 0, -40, 0);
        }
        public static void MouseWheelRight()
        {
            mouse_event(MOUSEEVENTF_HWHEEL, 0, 0, 40, 0);
        }

        public static void DoMouseClickLeft()
        {
            mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, _x, _y, 0, 0);
        }
        public static void DoMouseClickMiddle()
        {
            mouse_event(MOUSEEVENTF_LEFTUP, _x, _y, 0, 0);
        }
        
        public static void DoMouseClickRight()
        {
            mouse_event(MOUSEEVENTF_RIGHTDOWN | MOUSEEVENTF_RIGHTUP, _x, _y, 0, 0);
        }



        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void keybd_event(uint bVk, uint bScan, uint dwFlags, uint dwExtraInfo);

        private const int VK_LEFT = 0x25;

        private const int VK_LMENU = 0xA4;
        public static void LeftAlt()
        {
            keybd_event(VK_LMENU, 0, 0, 0);
        }
        public static void LeftArrow()
        {
            keybd_event(VK_LEFT, 0, 0, 0);
        }


    }

}
