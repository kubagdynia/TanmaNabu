using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using SFML.System;

namespace SFML.Window
{
    ////////////////////////////////////////////////////////////
    /// <summary>
    /// Enumeration of window creation styles
    /// </summary>
    ////////////////////////////////////////////////////////////
    [Flags]
    public enum Styles
    {
        /// <summary>No border / title bar (this flag and all others are mutually exclusive)</summary>
        None = 0,

        /// <summary>Title bar + fixed border</summary>
        Titlebar = 1 << 0,

        /// <summary>Titlebar + resizable border + maximize button</summary>
        Resize = 1 << 1,

        /// <summary>Titlebar + close button</summary>
        Close = 1 << 2,

        /// <summary>Fullscreen mode (this flag and all others are mutually exclusive))</summary>
        Fullscreen = 1 << 3,

        /// <summary>Default window style (titlebar + resize + close)</summary>
        Default = Titlebar | Resize | Close
    }

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// Window is a rendering window ; it can create a new window
    /// or connect to an existing one
    /// </summary>
    ////////////////////////////////////////////////////////////
    public class Window : ObjectBase
    {
        ////////////////////////////////////////////////////////////
        /// <summary>
        /// Create the window with default style and creation settings
        /// </summary>
        /// <param name="mode">Video mode to use</param>
        /// <param name="title">Title of the window</param>
        ////////////////////////////////////////////////////////////
        public Window(VideoMode mode, string title) :
            this(mode, title, Styles.Default, new ContextSettings(0, 0))
        {
        }

        ////////////////////////////////////////////////////////////
        /// <summary>
        /// Create the window with default creation settings
        /// </summary>
        /// <param name="mode">Video mode to use</param>
        /// <param name="title">Title of the window</param>
        /// <param name="style">Window style (Resize | Close by default)</param>
        ////////////////////////////////////////////////////////////
        public Window(VideoMode mode, string title, Styles style) :
            this(mode, title, style, new ContextSettings(0, 0))
        {
        }

        ////////////////////////////////////////////////////////////
        /// <summary>
        /// Create the window
        /// </summary>
        /// <param name="mode">Video mode to use</param>
        /// <param name="title">Title of the window</param>
        /// <param name="style">Window style (Resize | Close by default)</param>
        /// <param name="settings">Creation parameters</param>
        ////////////////////////////////////////////////////////////
        public Window(VideoMode mode, string title, Styles style, ContextSettings settings) : base(IntPtr.Zero)
        {
            // Copy the title to a null-terminated UTF-32 byte array
            byte[] titleAsUtf32 = Encoding.UTF32.GetBytes(title + '\0');

            unsafe
            {
                fixed (byte* titlePtr = titleAsUtf32)
                {
                    CPointer = sfWindow_createUnicode(mode, (IntPtr)titlePtr, style, ref settings);
                }
            }
        }

        ////////////////////////////////////////////////////////////
        /// <summary>
        /// Create the window from an existing control with default creation settings
        /// </summary>
        /// <param name="handle">Platform-specific handle of the control</param>
        ////////////////////////////////////////////////////////////
        public Window(IntPtr handle) :
            this(handle, new ContextSettings(0, 0))
        {
        }

        ////////////////////////////////////////////////////////////
        /// <summary>
        /// Create the window from an existing control
        /// </summary>
        /// <param name="Handle">Platform-specific handle of the control</param>
        /// <param name="settings">Creation parameters</param>
        ////////////////////////////////////////////////////////////
        public Window(IntPtr Handle, ContextSettings settings) :
            base(sfWindow_createFromHandle(Handle, ref settings))
        {
        }

        ////////////////////////////////////////////////////////////
        /// <summary>
        /// Tell whether or not the window is opened (ie. has been created).
        /// Note that a hidden window (Show(false))
        /// will still return true
        /// </summary>
        /// <returns>True if the window is opened</returns>
        ////////////////////////////////////////////////////////////
        public virtual bool IsOpen => sfWindow_isOpen(CPointer);

        ////////////////////////////////////////////////////////////
        /// <summary>
        /// Close (destroy) the window.
        /// The Window instance remains valid and you can call
        /// Create to recreate the window
        /// </summary>
        ////////////////////////////////////////////////////////////
        public virtual void Close() => sfWindow_close(CPointer);

        ////////////////////////////////////////////////////////////
        /// <summary>
        /// Display the window on screen
        /// </summary>
        ////////////////////////////////////////////////////////////
        public virtual void Display() => sfWindow_display(CPointer);

        ////////////////////////////////////////////////////////////
        /// <summary>
        /// Creation settings of the window
        /// </summary>
        ////////////////////////////////////////////////////////////
        public virtual ContextSettings Settings => sfWindow_getSettings(CPointer);

        ////////////////////////////////////////////////////////////
        /// <summary>
        /// Position of the window
        /// </summary>
        ////////////////////////////////////////////////////////////
        public virtual Vector2i Position
        {
            get => sfWindow_getPosition(CPointer);
            set => sfWindow_setPosition(CPointer, value);
        }

        ////////////////////////////////////////////////////////////
        /// <summary>
        /// Size of the rendering region of the window
        /// </summary>
        ////////////////////////////////////////////////////////////
        public virtual Vector2u Size
        {
            get => sfWindow_getSize(CPointer);
            set => sfWindow_setSize(CPointer, value);
        }

        ////////////////////////////////////////////////////////////
        /// <summary>
        /// Change the title of the window
        /// </summary>
        /// <param name="title">New title</param>
        ////////////////////////////////////////////////////////////
        public virtual void SetTitle(string title)
        {
            // Copy the title to a null-terminated UTF-32 byte array
            byte[] titleAsUtf32 = Encoding.UTF32.GetBytes(title + '\0');

            unsafe
            {
                fixed (byte* titlePtr = titleAsUtf32)
                {
                    sfWindow_setUnicodeTitle(CPointer, (IntPtr)titlePtr);
                }
            }
        }

        ////////////////////////////////////////////////////////////
        /// <summary>
        /// Change the window's icon
        /// </summary>
        /// <param name="width">Icon's width, in pixels</param>
        /// <param name="height">Icon's height, in pixels</param>
        /// <param name="pixels">Array of pixels, format must be RGBA 32 bits</param>
        ////////////////////////////////////////////////////////////
        public virtual void SetIcon(uint width, uint height, byte[] pixels)
        {
            unsafe
            {
                fixed (byte* PixelsPtr = pixels)
                {
                    sfWindow_setIcon(CPointer, width, height, PixelsPtr);
                }
            }
        }

        ////////////////////////////////////////////////////////////
        /// <summary>
        /// Show or hide the window
        /// </summary>
        /// <param name="visible">True to show the window, false to hide it</param>
        ////////////////////////////////////////////////////////////
        public virtual void SetVisible(bool visible) => sfWindow_setVisible(CPointer, visible);

        ////////////////////////////////////////////////////////////
        /// <summary>
        /// Show or hide the mouse cursor
        /// </summary>
        /// <param name="show">True to show, false to hide</param>
        ////////////////////////////////////////////////////////////
        public virtual void SetMouseCursorVisible(bool show) => sfWindow_setMouseCursorVisible(CPointer, show);

        ////////////////////////////////////////////////////////////
        /// <summary>
        /// Grab or release the mouse cursor
        /// </summary>
        /// <param name="grabbed">True to grab, false to release</param>
        /// 
        /// <remarks>
        /// If set, grabs the mouse cursor inside this window's client
        /// area so it may no longer be moved outside its bounds.
        /// Note that grabbing is only active while the window has
        /// focus and calling this function for fullscreen windows
        /// won't have any effect (fullscreen windows always grab the
        /// cursor).
        /// </remarks>
        ////////////////////////////////////////////////////////////
        public virtual void SetMouseCursorGrabbed(bool grabbed) => sfWindow_setMouseCursorGrabbed(CPointer, grabbed);

        ////////////////////////////////////////////////////////////
        /// <summary>
        /// Set the displayed cursor to a native system cursor
        /// </summary>
        /// <param name="enable">True to enable v-sync, false to deactivate</param>
        ////////////////////////////////////////////////////////////
        public virtual void SetMouseCursor(Cursor cursor) => sfWindow_setMouseCursor(CPointer, cursor.CPointer);

        ////////////////////////////////////////////////////////////
        /// <summary>
        /// Enable / disable vertical synchronization
        /// </summary>
        /// <param name="enable">True to enable v-sync, false to deactivate</param>
        ////////////////////////////////////////////////////////////
        public virtual void SetVerticalSyncEnabled(bool enable) => sfWindow_setVerticalSyncEnabled(CPointer, enable);

        ////////////////////////////////////////////////////////////
        /// <summary>
        /// Enable or disable automatic key-repeat.
        /// Automatic key-repeat is enabled by default
        /// </summary>
        /// <param name="enable">True to enable, false to disable</param>
        ////////////////////////////////////////////////////////////
        public virtual void SetKeyRepeatEnabled(bool enable) => sfWindow_setKeyRepeatEnabled(CPointer, enable);

        ////////////////////////////////////////////////////////////
        /// <summary>
        /// Activate the window as the current target
        /// for rendering
        /// </summary>
        /// <returns>True if operation was successful, false otherwise</returns>
        ////////////////////////////////////////////////////////////
        public virtual bool SetActive() => SetActive(true);

        ////////////////////////////////////////////////////////////
        /// <summary>
        /// Activate of deactivate the window as the current target
        /// for rendering
        /// </summary>
        /// <param name="active">True to activate, false to deactivate (true by default)</param>
        /// <returns>True if operation was successful, false otherwise</returns>
        ////////////////////////////////////////////////////////////
        public virtual bool SetActive(bool active) => sfWindow_setActive(CPointer, active);

        ////////////////////////////////////////////////////////////
        /// <summary>
        /// Limit the framerate to a maximum fixed frequency
        /// </summary>
        /// <param name="limit">Framerate limit, in frames per seconds (use 0 to disable limit)</param>
        ////////////////////////////////////////////////////////////
        public virtual void SetFramerateLimit(uint limit) => sfWindow_setFramerateLimit(CPointer, limit);

        ////////////////////////////////////////////////////////////
        /// <summary>
        /// Change the joystick threshold, ie. the value below which
        /// no move event will be generated
        /// </summary>
        /// <param name="threshold">New threshold, in range [0, 100]</param>
        ////////////////////////////////////////////////////////////
        public virtual void SetJoystickThreshold(float threshold) => sfWindow_setJoystickThreshold(CPointer, threshold);

        ////////////////////////////////////////////////////////////
        /// <summary>
        /// OS-specific handle of the window
        /// </summary>
        ////////////////////////////////////////////////////////////
        public virtual IntPtr SystemHandle => sfWindow_getSystemHandle(CPointer);

        ////////////////////////////////////////////////////////////
        /// <summary>
        /// Wait for a new event and dispatch it to the corresponding
        /// event handler
        /// </summary>
        ////////////////////////////////////////////////////////////
        public void WaitAndDispatchEvents()
        {
            Event e;
            if (WaitEvent(out e))
            {
                CallEventHandler(e);
            }
        }

        ////////////////////////////////////////////////////////////
        /// <summary>
        /// Call the event handlers for each pending event
        /// </summary>
        ////////////////////////////////////////////////////////////
        public void DispatchEvents()
        {
            Event e;
            while (PollEvent(out e))
            {
                CallEventHandler(e);
            }
        }

        ////////////////////////////////////////////////////////////
        /// <summary>
        /// Request the current window to be made the active
        /// foreground window
        /// </summary>
        ////////////////////////////////////////////////////////////
        public virtual void RequestFocus() => sfWindow_requestFocus(CPointer);

        ////////////////////////////////////////////////////////////
        /// <summary>
        /// Check whether the window has the input focus
        /// </summary>
        /// <returns>True if the window has focus, false otherwise</returns>
        ////////////////////////////////////////////////////////////
        public virtual bool HasFocus() => sfWindow_hasFocus(CPointer);

        ////////////////////////////////////////////////////////////
        /// <summary>
        /// Provide a string describing the object
        /// </summary>
        /// <returns>String description of the object</returns>
        ////////////////////////////////////////////////////////////
        public override string ToString() => $"[Window] Size({Size}) Position({Position}) Settings({Settings})";

        ////////////////////////////////////////////////////////////
        /// <summary>
        /// Constructor for derived classes
        /// </summary>
        /// <param name="cPointer">Pointer to the internal object in the C API</param>
        /// <param name="dummy">Internal hack :)</param>
        ////////////////////////////////////////////////////////////
        protected Window(IntPtr cPointer, int dummy) :
            base(cPointer)
        {
            // TODO : find a cleaner way of separating this constructor from Window(IntPtr handle)
        }

        ////////////////////////////////////////////////////////////
        /// <summary>
        /// Internal function to get the next event (non-blocking)
        /// </summary>
        /// <param name="eventToFill">Variable to fill with the raw pointer to the event structure</param>
        /// <returns>True if there was an event, false otherwise</returns>
        ////////////////////////////////////////////////////////////
        protected virtual bool PollEvent(out Event eventToFill) => sfWindow_pollEvent(CPointer, out eventToFill);

        ////////////////////////////////////////////////////////////
        /// <summary>
        /// Internal function to get the next event (blocking)
        /// </summary>
        /// <param name="eventToFill">Variable to fill with the raw pointer to the event structure</param>
        /// <returns>False if any error occured</returns>
        ////////////////////////////////////////////////////////////
        public virtual bool WaitEvent(out Event eventToFill) => sfWindow_waitEvent(CPointer, out eventToFill);

        ////////////////////////////////////////////////////////////
        /// <summary>
        /// Internal function to get the mouse position relative to the window.
        /// This function is protected because it is called by another class of
        /// another module, it is not meant to be called by users.
        /// </summary>
        /// <returns>Relative mouse position</returns>
        ////////////////////////////////////////////////////////////
        public virtual Vector2i InternalGetMousePosition() => sfMouse_getPosition(CPointer);

        ////////////////////////////////////////////////////////////
        /// <summary>
        /// Internal function to set the mouse position relative to the window.
        /// This function is protected because it is called by another class of
        /// another module, it is not meant to be called by users.
        /// </summary>
        /// <param name="position">Relative mouse position</param>
        ////////////////////////////////////////////////////////////
        public virtual void InternalSetMousePosition(Vector2i position) => sfMouse_setPosition(position, CPointer);

        ////////////////////////////////////////////////////////////
        /// <summary>
        /// Internal function to get the touch position relative to the window.
        /// This function is protected because it is called by another class of
        /// another module, it is not meant to be called by users.
        /// </summary>
        /// <param name="Finger">Finger index</param>
        /// <returns>Relative touch position</returns>
        ////////////////////////////////////////////////////////////
        public virtual Vector2i InternalGetTouchPosition(uint Finger) => sfTouch_getPosition(Finger, CPointer);

        ////////////////////////////////////////////////////////////
        /// <summary>
        /// Handle the destruction of the object
        /// </summary>
        /// <param name="disposing">Is the GC disposing the object, or is it an explicit call ?</param>
        ////////////////////////////////////////////////////////////
        protected override void Destroy(bool disposing) => sfWindow_destroy(CPointer);

        ////////////////////////////////////////////////////////////
        /// <summary>
        /// Call the event handler for the given event
        /// </summary>
        /// <param name="e">Event to dispatch</param>
        ////////////////////////////////////////////////////////////
        private void CallEventHandler(Event e)
        {
            switch (e.Type)
            {
                case EventType.Closed:
                    Closed?.Invoke(this, EventArgs.Empty);
                    break;

                case EventType.GainedFocus:
                    GainedFocus?.Invoke(this, EventArgs.Empty);
                    break;

                case EventType.JoystickButtonPressed:
                    JoystickButtonPressed?.Invoke(this, new JoystickButtonEventArgs(e.JoystickButton));
                    break;

                case EventType.JoystickButtonReleased:
                    JoystickButtonReleased?.Invoke(this, new JoystickButtonEventArgs(e.JoystickButton));
                    break;

                case EventType.JoystickMoved:
                    JoystickMoved?.Invoke(this, new JoystickMoveEventArgs(e.JoystickMove));
                    break;

                case EventType.JoystickConnected:
                    JoystickConnected?.Invoke(this, new JoystickConnectEventArgs(e.JoystickConnect));
                    break;

                case EventType.JoystickDisconnected:
                    JoystickDisconnected?.Invoke(this, new JoystickConnectEventArgs(e.JoystickConnect));
                    break;

                case EventType.KeyPressed:
                    KeyPressed?.Invoke(this, new KeyEventArgs(e.Key));
                    break;

                case EventType.KeyReleased:
                    KeyReleased?.Invoke(this, new KeyEventArgs(e.Key));
                    break;

                case EventType.LostFocus:
                    LostFocus?.Invoke(this, EventArgs.Empty);
                    break;

                case EventType.MouseButtonPressed:
                    MouseButtonPressed?.Invoke(this, new MouseButtonEventArgs(e.MouseButton));
                    break;

                case EventType.MouseButtonReleased:
                    MouseButtonReleased?.Invoke(this, new MouseButtonEventArgs(e.MouseButton));
                    break;

                case EventType.MouseEntered:
                    MouseEntered?.Invoke(this, EventArgs.Empty);
                    break;

                case EventType.MouseLeft:
                    MouseLeft?.Invoke(this, EventArgs.Empty);
                    break;

                case EventType.MouseMoved:
                    MouseMoved?.Invoke(this, new MouseMoveEventArgs(e.MouseMove));
                    break;

                case EventType.MouseWheelMoved:
                    MouseWheelMoved?.Invoke(this, new MouseWheelEventArgs(e.MouseWheel));
                    break;

                case EventType.MouseWheelScrolled:
                    MouseWheelScrolled?.Invoke(this, new MouseWheelScrollEventArgs(e.MouseWheelScroll));
                    break;

                case EventType.Resized:
                    Resized?.Invoke(this, new SizeEventArgs(e.Size));
                    break;

                case EventType.TextEntered:
                    TextEntered?.Invoke(this, new TextEventArgs(e.Text));
                    break;

                case EventType.TouchBegan:
                    TouchBegan?.Invoke(this, new TouchEventArgs(e.Touch));
                    break;

                case EventType.TouchMoved:
                    TouchMoved?.Invoke(this, new TouchEventArgs(e.Touch));
                    break;

                case EventType.TouchEnded:
                    TouchEnded?.Invoke(this, new TouchEventArgs(e.Touch));
                    break;

                case EventType.SensorChanged:
                    SensorChanged?.Invoke(this, new SensorEventArgs(e.Sensor));
                    break;

                default:
                    break;
            }
        }

        /// <summary>Event handler for the Closed event</summary>
        public event EventHandler Closed = null;

        /// <summary>Event handler for the Resized event</summary>
        public event EventHandler<SizeEventArgs> Resized = null;

        /// <summary>Event handler for the LostFocus event</summary>
        public event EventHandler LostFocus = null;

        /// <summary>Event handler for the GainedFocus event</summary>
        public event EventHandler GainedFocus = null;

        /// <summary>Event handler for the TextEntered event</summary>
        public event EventHandler<TextEventArgs> TextEntered = null;

        /// <summary>Event handler for the KeyPressed event</summary>
        public event EventHandler<KeyEventArgs> KeyPressed = null;

        /// <summary>Event handler for the KeyReleased event</summary>
        public event EventHandler<KeyEventArgs> KeyReleased = null;

        /// <summary>Event handler for the MouseWheelMoved event</summary>
        [Obsolete("MouseWheelMoved is deprecated, please use MouseWheelScrolled instead")]
        public event EventHandler<MouseWheelEventArgs> MouseWheelMoved = null;

        /// <summary>Event handler for the MouseWheelScrolled event</summary>
        public event EventHandler<MouseWheelScrollEventArgs> MouseWheelScrolled = null;

        /// <summary>Event handler for the MouseButtonPressed event</summary>
        public event EventHandler<MouseButtonEventArgs> MouseButtonPressed = null;

        /// <summary>Event handler for the MouseButtonReleased event</summary>
        public event EventHandler<MouseButtonEventArgs> MouseButtonReleased = null;

        /// <summary>Event handler for the MouseMoved event</summary>
        public event EventHandler<MouseMoveEventArgs> MouseMoved = null;

        /// <summary>Event handler for the MouseEntered event</summary>
        public event EventHandler MouseEntered = null;

        /// <summary>Event handler for the MouseLeft event</summary>
        public event EventHandler MouseLeft = null;

        /// <summary>Event handler for the JoystickButtonPressed event</summary>
        public event EventHandler<JoystickButtonEventArgs> JoystickButtonPressed = null;

        /// <summary>Event handler for the JoystickButtonReleased event</summary>
        public event EventHandler<JoystickButtonEventArgs> JoystickButtonReleased = null;

        /// <summary>Event handler for the JoystickMoved event</summary>
        public event EventHandler<JoystickMoveEventArgs> JoystickMoved = null;

        /// <summary>Event handler for the JoystickConnected event</summary>
        public event EventHandler<JoystickConnectEventArgs> JoystickConnected = null;

        /// <summary>Event handler for the JoystickDisconnected event</summary>
        public event EventHandler<JoystickConnectEventArgs> JoystickDisconnected = null;

        /// <summary>Event handler for the TouchBegan event</summary>
        public event EventHandler<TouchEventArgs> TouchBegan = null;

        /// <summary>Event handler for the TouchMoved event</summary>
        public event EventHandler<TouchEventArgs> TouchMoved = null;

        /// <summary>Event handler for the TouchEnded event</summary>
        public event EventHandler<TouchEventArgs> TouchEnded = null;

        /// <summary>Event handler for the SensorChanged event</summary>
        public event EventHandler<SensorEventArgs> SensorChanged = null;

        #region Imports

        [DllImport(SfmlSettings.CsfmlWindowPath, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern IntPtr sfWindow_create(VideoMode Mode, string Title, Styles Style, ref ContextSettings Params);

        [DllImport(SfmlSettings.CsfmlWindowPath, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern IntPtr sfWindow_createUnicode(VideoMode Mode, IntPtr Title, Styles Style, ref ContextSettings Params);

        [DllImport(SfmlSettings.CsfmlWindowPath, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern IntPtr sfWindow_createFromHandle(IntPtr Handle, ref ContextSettings Params);

        [DllImport(SfmlSettings.CsfmlWindowPath, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern void sfWindow_destroy(IntPtr CPointer);

        [DllImport(SfmlSettings.CsfmlWindowPath, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern bool sfWindow_isOpen(IntPtr CPointer);

        [DllImport(SfmlSettings.CsfmlWindowPath, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern void sfWindow_close(IntPtr CPointer);

        [DllImport(SfmlSettings.CsfmlWindowPath, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern bool sfWindow_pollEvent(IntPtr CPointer, out Event Evt);

        [DllImport(SfmlSettings.CsfmlWindowPath, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern bool sfWindow_waitEvent(IntPtr CPointer, out Event Evt);

        [DllImport(SfmlSettings.CsfmlWindowPath, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern void sfWindow_display(IntPtr CPointer);

        [DllImport(SfmlSettings.CsfmlWindowPath, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern ContextSettings sfWindow_getSettings(IntPtr CPointer);

        [DllImport(SfmlSettings.CsfmlWindowPath, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern Vector2i sfWindow_getPosition(IntPtr CPointer);

        [DllImport(SfmlSettings.CsfmlWindowPath, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern void sfWindow_setPosition(IntPtr CPointer, Vector2i position);

        [DllImport(SfmlSettings.CsfmlWindowPath, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern Vector2u sfWindow_getSize(IntPtr CPointer);

        [DllImport(SfmlSettings.CsfmlWindowPath, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern void sfWindow_setSize(IntPtr CPointer, Vector2u size);

        [DllImport(SfmlSettings.CsfmlWindowPath, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern void sfWindow_setTitle(IntPtr CPointer, string title);

        [DllImport(SfmlSettings.CsfmlWindowPath, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern void sfWindow_setUnicodeTitle(IntPtr CPointer, IntPtr title);

        [DllImport(SfmlSettings.CsfmlWindowPath, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        unsafe static extern void sfWindow_setIcon(IntPtr CPointer, uint Width, uint Height, byte* Pixels);

        [DllImport(SfmlSettings.CsfmlWindowPath, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern void sfWindow_setVisible(IntPtr CPointer, bool visible);

        [DllImport(SfmlSettings.CsfmlWindowPath, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern void sfWindow_setMouseCursorVisible(IntPtr CPointer, bool Show);

        [DllImport(SfmlSettings.CsfmlWindowPath, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern void sfWindow_setMouseCursorGrabbed(IntPtr CPointer, bool grabbed);

        [DllImport(SfmlSettings.CsfmlWindowPath, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern void sfWindow_setMouseCursor(IntPtr CPointer, IntPtr cursor);

        [DllImport(SfmlSettings.CsfmlWindowPath, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern void sfWindow_setVerticalSyncEnabled(IntPtr CPointer, bool Enable);

        [DllImport(SfmlSettings.CsfmlWindowPath, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern void sfWindow_setKeyRepeatEnabled(IntPtr CPointer, bool Enable);

        [DllImport(SfmlSettings.CsfmlWindowPath, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern bool sfWindow_setActive(IntPtr CPointer, bool Active);

        [DllImport(SfmlSettings.CsfmlWindowPath, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern void sfWindow_setFramerateLimit(IntPtr CPointer, uint Limit);

        [DllImport(SfmlSettings.CsfmlWindowPath, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern uint sfWindow_getFrameTime(IntPtr CPointer);

        [DllImport(SfmlSettings.CsfmlWindowPath, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern void sfWindow_setJoystickThreshold(IntPtr CPointer, float Threshold);

        [DllImport(SfmlSettings.CsfmlWindowPath, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern IntPtr sfWindow_getSystemHandle(IntPtr CPointer);

        [DllImport(SfmlSettings.CsfmlWindowPath, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern void sfWindow_requestFocus(IntPtr CPointer);

        [DllImport(SfmlSettings.CsfmlWindowPath, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern bool sfWindow_hasFocus(IntPtr CPointer);

        [DllImport(SfmlSettings.CsfmlWindowPath, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern Vector2i sfMouse_getPosition(IntPtr CPointer);

        [DllImport(SfmlSettings.CsfmlWindowPath, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern void sfMouse_setPosition(Vector2i position, IntPtr CPointer);

        [DllImport(SfmlSettings.CsfmlWindowPath, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern Vector2i sfTouch_getPosition(uint Finger, IntPtr RelativeTo);

        #endregion
    }
}
