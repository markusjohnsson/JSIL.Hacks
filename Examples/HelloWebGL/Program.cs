using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JSIL.Dom;
using JSIL.WebGL;

namespace HelloWebGL
{
    class PositionBuffer
    {
        public GLBuffer Buffer { get; set; }
        public int ItemSize { get; set; }
        public int NumItems { get; set; }
    }

    public class Program
    {
        private static WebGLContext gl;
        private static PositionBuffer triangleVertexPositionBuffer;
        private static PositionBuffer squareVertexPositionBuffer;
        private static double viewportWidth;
        private static double viewportHeight;
        private static GLShaderProgram shaderProgram;
        private static GLAttributeLocation vertexPositionAttribute;
        private static GLUniformLocation mvMatrixUniform;
        private static GLUniformLocation pMatrixUniform;
        private static Matrix4 pMatrix;
        private static Matrix4 mvMatrix;

        public static void Main()
        {
            var target = Element.GetById("target");
            var canvas = new Canvas();
            target.AppendChild(canvas);

            initGL(canvas);
            initShaders();
            initBuffers();

            gl.ClearColor(0.0, 0.0, 0.0, 1.0);
            gl.Enable(gl.DepthTest);

            drawScene();
        }

        private static void drawScene()
        {
            gl.Viewport(0, 0, viewportWidth, viewportHeight);
            gl.Clear(gl.ColorBufferBit | gl.DepthBufferBit);
            pMatrix = Matrix4.Perspective(45, viewportWidth / viewportHeight, 0.1, 100.0);

            // Draw triangle
            mvMatrix = Matrix4.Translate(-1.5, 0.0, -7.0);
            gl.BindBuffer(gl.ArrayBuffer, triangleVertexPositionBuffer.Buffer);
            gl.VertexAttributePointer(vertexPositionAttribute, triangleVertexPositionBuffer.ItemSize, gl.Float, false, 0, 0);
            setMatrixUniforms();
            gl.DrawArrays(gl.Triangles, 0, triangleVertexPositionBuffer.NumItems);

            // Draw square
            mvMatrix = Matrix4.Translate(1.5, 0.0, -7.0);
            gl.BindBuffer(gl.ArrayBuffer, squareVertexPositionBuffer.Buffer);
            gl.VertexAttributePointer(vertexPositionAttribute, squareVertexPositionBuffer.ItemSize, gl.Float, false, 0, 0);
            setMatrixUniforms();
            gl.DrawArrays(gl.TriangleStrip, 0, squareVertexPositionBuffer.NumItems);
        }

        private static void setMatrixUniforms()
        {
            gl.UniformMatrix4fv(pMatrixUniform, false, pMatrix.ToArray());
            gl.UniformMatrix4fv(mvMatrixUniform, false, mvMatrix.ToArray());
        }

        private static void initBuffers()
        {
            triangleVertexPositionBuffer = new PositionBuffer();
            triangleVertexPositionBuffer.Buffer = gl.CreateBuffer();
            gl.BindBuffer(gl.ArrayBuffer, triangleVertexPositionBuffer.Buffer);
            var vertices = new [] {
                 0.0,  1.0,  0.0,
                -1.0, -1.0,  0.0,
                 1.0, -1.0,  0.0
            };
            gl.BufferData(gl.ArrayBuffer, Float32Array.Create(vertices), gl.StaticDraw);
            triangleVertexPositionBuffer.ItemSize = 3;
            triangleVertexPositionBuffer.NumItems = 3;

            squareVertexPositionBuffer = new PositionBuffer();
            squareVertexPositionBuffer.Buffer = gl.CreateBuffer();
            gl.BindBuffer(gl.ArrayBuffer, squareVertexPositionBuffer.Buffer);
            vertices = new [] {
                 1.0,  1.0,  0.0,
                -1.0,  1.0,  0.0,
                 1.0, -1.0,  0.0,
                -1.0, -1.0,  0.0
            };
            gl.BufferData(gl.ArrayBuffer, Float32Array.Create(vertices), gl.StaticDraw);
            squareVertexPositionBuffer.ItemSize = 3;
            squareVertexPositionBuffer.NumItems = 4;
        }

        private static void initShaders()
        {
            var fragmentShader = GetShader(gl, "shader-fs");
            var vertexShader = GetShader(gl, "shader-vs");

            shaderProgram = gl.CreateProgram();
            gl.AttachShader(shaderProgram, vertexShader);
            gl.AttachShader(shaderProgram, fragmentShader);
            gl.LinkProgram(shaderProgram);

            if (!gl.GetProgramParameter(shaderProgram, gl.LinkStatus))
            {
                throw new Exception("Could not initialize shaders");
            }

            gl.UseProgram(shaderProgram);

            vertexPositionAttribute = gl.GetAttributeLocation(shaderProgram, "aVertexPosition");
            gl.EnableVertexAttributeArray(vertexPositionAttribute);

            pMatrixUniform = gl.GetUniformLocation(shaderProgram, "uPMatrix");
            mvMatrixUniform = gl.GetUniformLocation(shaderProgram, "uMVMatrix");
        }

        private static GLShader GetShader(WebGLContext gl, string id)
        {
            var shaderScript = Element.GetById(id);

            var str = shaderScript.TextContent;
            if (str == "" || str == null)
            {
                var k = shaderScript.FirstChild;
                while (k != null)
                {
                    if (k.NodeType == 3)
                        str += k.TextContent;
                    k = k.NextSibling;
                }
            }

            GLShader shader;
            if (shaderScript.GetAttributeValue("type") == "x-shader/x-fragment")
            {
                shader = gl.CreateShader(gl.FragmentShader);
            }
            else if (shaderScript.GetAttributeValue("type") == "x-shader/x-vertex")
            {
                shader = gl.CreateShader(gl.VertexShader);
            }
            else
            {
                return null;
            }

            gl.ShaderSource(shader, str);
            gl.CompileShader(shader);

            if (!gl.GetShaderParameter(shader, gl.CompileStatus))
            {
                throw new Exception("Shader compilation failed");
            }

            return shader;
        }

        private static void initGL(Canvas canvas)
        {
            gl = canvas.GetWebGLContext();
            viewportWidth = canvas.Width = 500;
            viewportHeight = canvas.Height = 500;
        }
    }
}
