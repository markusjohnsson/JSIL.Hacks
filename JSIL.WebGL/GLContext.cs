using System;
using JSIL.Dom;
using JSIL.Meta;

namespace JSIL.WebGL
{

    public static class CanvasEx
    {
        public static WebGLContext GetWebGLContext(this Canvas canvas)
        {
            return WebGLContext.GetContext(canvas);
        }
    }

    public class WebGLContext
    {
        [JSReplacement("$this.gl.COLOR_BUFFER_BIT")]
        public int ColorBufferBit { get; private set; }

        [JSReplacement("$this.gl.DEPTH_BUFFER_BIT")]
        public int DepthBufferBit { get; private set; }

        private object gl;

        internal WebGLContext(object context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            gl = context;
        }

        internal static WebGLContext GetContext(Canvas canvas)
        {
            return new WebGLContext(Verbatim.Expression("canvas._element.getContext('experimental-webgl')"));
        }

        [JSReplacement("$this.gl.clearColor($r, $g, $b, $a)")]
        public void ClearColor(double r, double g, double b, double a) { }

        [JSReplacement("$this.gl.viewport($x, $y, $w, $h)")]
        public void ViewPort(double x, double y, double w, double h) { }

        [JSReplacement("$this.gl.clear($flags)")]
        public void Clear(int flags) { }

        [JSReplacement("$this.gl.createBuffer()")]
        public GLBuffer CreateBuffer() { throw new NotSupportedException(); }

        [JSReplacement("$this.gl.ARRAY_BUFFER")]
        public GLBufferType ArrayBuffer { get; private set; }

        [JSReplacement("$this.gl.STATIC_DRAW")]
        public GLBufferUsage StaticDraw { get; private set; }

        [JSReplacement("$this.gl.bindBuffer($bufferType, $buffer)")]
        public void BindBuffer(GLBufferType bufferType, GLBuffer buffer) { }

        [JSReplacement("$this.gl.bufferData($bufferType, $array, $bufferUsage)")]
        public void BufferData(GLBufferType bufferType, Float32Array array, GLBufferUsage bufferUsage) { }

        [JSReplacement("$this.gl.FLOAT")]
        public GLType Float { get; private set; }

        [JSReplacement("$this.gl.vertexAttribPointer($attributeLocation, $itemSize, $type, $normalized, $stride, $offset)")]
        public void VertexAttributePointer(GLAttributeLocation attributeLocation, int itemSize, GLType type, bool normalized, int stride, int offset) { }

        [JSReplacement("$this.gl.FRAGMENT_SHADER")]
        public GLShaderType FragmentShader { get; private set; }

        [JSReplacement("$this.gl.VERTEX_SHADER")]
        public GLShaderType VertexShader { get; private set; }

        [JSReplacement("$this.gl.createShader($shaderType)")]
        public GLShader CreateShader(GLShaderType shaderType)
        {
            throw new NotImplementedException();
        }

        [JSReplacement("$this.gl.shaderSource($shader, $source)")]
        public void ShaderSource(GLShader shader, string source) { }

        [JSReplacement("$this.gl.compileShader($shader)")]
        public void CompileShader(GLShader shader) { }

        [JSReplacement("$this.gl.createProgram()")]
        public GLShaderProgram CreateProgram()
        {
            throw new NotImplementedException();
        }

        [JSReplacement("$this.gl.attachShader($program, $shader)")]
        public void AttachShader(GLShaderProgram program, GLShader shader) { }

        [JSReplacement("$this.gl.linkProgram($program)")]
        public void LinkProgram(GLShaderProgram program) { }

        [JSReplacement("$this.gl.useProgram($program)")]
        public void UseProgram(GLShaderProgram program) { }

        [JSReplacement("$this.gl.getAttribLocation($program, $attribute)")]
        public GLAttributeLocation GetAttributeLocation(GLShaderProgram program, string attribute) 
        {
            throw new NotImplementedException();
        }

        [JSReplacement("$this.gl.getUniformLocation($program, $uniform)")]
        public GLUniformLocation GetUniformLocation(GLShaderProgram program, string uniform)
        {
            throw new NotImplementedException();
        }

        [JSReplacement("$this.gl.enableVertexAttribArray($attributeLocation)")]
        public void EnableVertexAttributeArray(GLAttributeLocation attributeLocation) { }

        [JSReplacement("$this.gl.TRIANGLE_STRIP")]
        public GLDrawMode TriangleStrip { get; private set; }

        [JSReplacement("$this.gl.TRIANGLES")]
        public GLDrawMode Triangles { get; private set; }

        [JSReplacement("$this.gl.drawArrays($drawMode, $first, $count)")]
        public void DrawArrays(GLDrawMode drawMode, int first, int count) { }

        [JSReplacement("$this.gl.uniformMatrix4fv($uniformLocation, $transpose, $value)")]
        public void UniformMatrix4fv(GLUniformLocation uniformLocation, bool transpose, float[] value) { }
    }
}
