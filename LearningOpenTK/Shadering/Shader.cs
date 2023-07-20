using OpenTK.Graphics.ES20;
using System;
using System.IO;
using System.Numerics;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace LearningOpenTK.Shadering
{
    public class Shader
    {
        public int id { get; private set; }

        public Shader(string vertexShaderLocation, string fragmentShaderLocation)
        {
            id = LoadShaderProgram(vertexShaderLocation, fragmentShaderLocation);
        }

        public void Use()
        {
            GL.UseProgram(id);
        }

        public void UnBind()
        {
            GL.UseProgram(0);
        }

        public void SetInt(string name, int data)
        {
            Use();
            int location = GL.GetUniformLocation(id, name);
            GL.Uniform1(location, data);
        }

        public void SetFloat(string name, float data)
        {
            Use();
            int location = GL.GetUniformLocation(id, name);
            GL.Uniform1(location, data);
        }

        public void SetMatrix(string name, Matrix4 matrix)
        {
            Use();
            int location = GL.GetUniformLocation(id, name);
            GL.UniformMatrix4(location, false, ref matrix);
        }

        private int LoadShader(string location, ShaderType type)
        {
            int shaderId = GL.CreateShader(type);
            GL.ShaderSource(shaderId, File.ReadAllText(location));
            GL.CompileShader(shaderId);

            string infoLog = GL.GetShaderInfoLog(shaderId);
            if (!string.IsNullOrEmpty(infoLog))
            {
                throw new Exception(infoLog);
            }

            return shaderId;
        }

        private int LoadShaderProgram(string vertexShaderLocation, string fragmentShaderLocation)
        {
            int shaderPorgramId = GL.CreateProgram();

            int vertexShader = LoadShader(vertexShaderLocation, ShaderType.VertexShader);
            int fragmentShader = LoadShader(fragmentShaderLocation, ShaderType.FragmentShader);

            GL.AttachShader(shaderPorgramId, vertexShader);
            GL.AttachShader(shaderPorgramId, fragmentShader);
            GL.LinkProgram(shaderPorgramId);

            GL.DetachShader(shaderPorgramId, vertexShader);
            GL.DetachShader(shaderPorgramId, fragmentShader);
            GL.DeleteShader(vertexShader);
            GL.DeleteShader(fragmentShader);

            string infoLog = GL.GetProgramInfoLog(shaderPorgramId);
            if (!string.IsNullOrEmpty(infoLog))
            {
                throw new Exception(infoLog);
            }

            return shaderPorgramId;
        }
    }
}
