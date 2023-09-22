using OpenTK.Windowing.GraphicsLibraryFramework;

namespace TheBookOfShaders
{
    internal class Camera
    {
        internal Vector3 Position { get; set; }
        internal float AspectRatio { get; set; }
        internal float Speed { get; set; } = 2.5f;
        internal float Sensitivity { get; set; } = 0.1f;

        /// <summary>
        /// In radians
        /// </summary>
        private float fov;
        /// <summary>
        /// In radians
        /// </summary>
        private float zoom;
        
        internal Vector3 Front { get; private set; } = -Vector3.UnitZ;
        internal Vector3 Up { get; private set; } = Vector3.UnitY;
        internal Vector3 Right { get; private set; } 

        /// <summary>
        /// In radians
        /// </summary>
        private float yaw = -MathHelper.PiOver2;
        /// <summary>
        /// In radians
        /// </summary>
        private float pitch = 0;

        /// <summary>
        /// In Degrees 
        /// </summary>
        internal float Fov
        {
            get => MathHelper.RadiansToDegrees(fov);
            set { fov = MathHelper.DegreesToRadians(MathHelper.Clamp(value, 1, 120)); }
        }
        /// <summary>
        /// In Degrees 
        /// </summary>
        internal float Zoom
        {
            get => MathHelper.RadiansToDegrees(zoom);
            set { zoom = MathHelper.DegreesToRadians(MathHelper.Clamp(value, 1, Fov - 1)); }
        }

        /// <summary>
        /// In degress
        /// </summary>
        internal float Yaw
        {
            get => MathHelper.RadiansToDegrees(yaw);
            set { yaw = MathHelper.DegreesToRadians(value); }
        }
        /// <summary>
        /// In degress
        /// </summary>
        internal float Pitch
        {
            get => MathHelper.RadiansToDegrees(pitch);
            set { pitch = MathHelper.DegreesToRadians(value); }
        }

        internal Camera(Vector3 position, float aspectRatio, float fov = 90)
        {
            this.Position = position;
            this.AspectRatio = aspectRatio;
            this.Fov = fov;
        }

        internal Matrix4 GetProjectionMatrix()
        {
            return Matrix4.CreatePerspectiveFieldOfView(fov - zoom, AspectRatio, 0.1f, 100.0f);
        }

        internal Matrix4 GetViewMatrix()
        {
            return Matrix4.LookAt(Position, Position + Front, Up);
        }

        internal Matrix4 GetViewMatrix(Vector3 LookingAt)
        {
            return Matrix4.LookAt(Position, LookingAt, Up);
        }

        internal void UpdateKeys(MouseState mouse, KeyboardState keyboard, float time)
        {
            Yaw += MathHelper.Clamp(mouse.Delta.X * Sensitivity, -89, 89);
            Pitch += MathHelper.Clamp(-mouse.Delta.Y * Sensitivity, -89, 89);

            Vector3 Direction = new Vector3();
            Direction.X = MathF.Cos(yaw) * MathF.Cos(pitch);
            Direction.Y = MathF.Sin(pitch);
            Direction.Z = MathF.Sin(yaw) * MathF.Cos(pitch);
            
            Front = Vector3.Normalize(Direction);
            Right = Vector3.Normalize(Vector3.Cross(Front, Vector3.UnitY));
            Up = Vector3.Cross(Right, Front);

            Zoom += mouse.ScrollDelta.Y;

            if (keyboard.IsKeyDown(Keys.Space))
            {
                Position += new Vector3(0.0f, Speed * time, 0.0f);
            }
            if (keyboard.IsKeyDown(Keys.LeftControl))
            {
                Position -= new Vector3(0.0f, Speed * time, 0.0f);
            }
            if (keyboard.IsKeyDown(Keys.W))
            {
                Position += Speed * time * Front;
            }
            if (keyboard.IsKeyDown(Keys.S))
            {
                Position -= Speed * time * Front;
            }
            if (keyboard.IsKeyDown(Keys.A))
            {
                Position -= Speed * time * Vector3.Normalize(Vector3.Cross(Front, Up));
            }
            if (keyboard.IsKeyDown(Keys.D))
            {
                Position += Speed * time * Vector3.Normalize(Vector3.Cross(Front, Up));
            }
        }
    }
}
