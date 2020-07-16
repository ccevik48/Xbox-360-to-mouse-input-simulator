# Xbox-360-to-mouse-input-simulator
Simulate mouse movement and behavior using C#


To use this repository, clone it and open it in Visual Studio. You will need to install the XboxController package https://github.com/BrandonPotter/XBoxController and the InputSimulator package https://archive.codeplex.com/?p=inputsimulator. 

For this project, a loop is executed using threading, which sleeps for 10 ms. Because of this, a buffer is used for most inputs to prevent pressing a button multiple times when pressed. In this implementation, the 'A' button is left click, LB is hold left click, 'B' is right click, left stick is mouse movement, right stick is scroll movement, and left and right on the D-pad is browser forward and back for convenience, meaning D-pad up/down, 'X', 'Y', RB, LT, RT, and both thumbstick down buttons are not mapped yet. Using ImputSimulator or by creating DllImport functions, the buttons may be mapped to potentially any key. 
