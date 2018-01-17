﻿using CSharpGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShaderDefineClipPlane
{
    partial class TransparentPlaneNode : PickableNode
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static TransparentPlaneNode Create()
        {
            var model = new TransparentPlaneModel();
            var vs = new VertexShader(vertexCode);
            var fs = new FragmentShader(fragmentCode);
            var array = new ShaderArray(vs, fs);
            var map = new AttributeMap();
            map.Add("inPosition", TransparentPlaneModel.strPosition);
            map.Add("inColor", TransparentPlaneModel.strColor);
            var builder = new RenderMethodBuilder(array, map);
            var node = new TransparentPlaneNode(model, TransparentPlaneModel.strPosition, builder);
            node.Initialize();
            node.ModelSize = model.ModelSize;

            return node;
        }

        private TransparentPlaneNode(IBufferSource model, string positionNameInIBufferSource, params RenderMethodBuilder[] builders) : base(model, positionNameInIBufferSource, builders) { }

        public override void RenderBeforeChildren(RenderEventArgs arg)
        {
            if (!this.IsInitialized) { this.Initialize(); }

            ICamera camera = arg.CameraStack.Peek();
            mat4 projection = camera.GetProjectionMatrix();
            mat4 view = camera.GetViewMatrix();
            mat4 model = this.GetModelMatrix();

            var method = this.RenderUnit.Methods[0]; // the only render unit in this node.
            ShaderProgram program = method.Program;
            program.SetUniform("projectionMat", projection);
            program.SetUniform("viewMat", view);
            program.SetUniform("modelMat", model);

            method.Render();
        }

        public override void RenderAfterChildren(RenderEventArgs arg)
        {
        }
    }
}
