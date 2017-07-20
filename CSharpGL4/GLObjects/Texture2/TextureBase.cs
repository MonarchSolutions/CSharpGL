﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSharpGL.Texture2
{
    public abstract partial class TextureBase : IDisposable
    {
        /// <summary>
        /// binding target of this texture.
        /// </summary>
        public TextureTarget Target { get; protected set; }

        /// <summary>
        /// texture's id/name from glGenTextures().
        /// 纹理名（用于标识一个纹理，由OpenGL指定）。
        /// </summary>
        protected uint[] id = new uint[1];

        /// <summary>
        /// texture's id/name from glGenTextures().
        /// 纹理名（用于标识一个纹理，由OpenGL指定）。
        /// </summary>
        public uint Id { get { return this.id[0]; } }

        ///// <summary>
        /////
        ///// </summary>
        //public bool UseMipmap { get; private set; }

        public TextureBase(TextureTarget target, TexStorageBase storage)
        {
            this.Target = target;
            this.Storage = storage;
        }
        /// <summary>
        ///
        /// </summary>
        public void Bind()
        {
            GL.Instance.BindTexture((uint)this.Target, this.Id);
        }

        /// <summary>
        ///
        /// </summary>
        public void Unbind()
        {
            GL.Instance.BindTexture((uint)this.Target, 0);
        }

        private bool initialized = false;

        /// <summary>
        /// resources(bitmap etc.) can be disposed  after this initialization.
        /// </summary>
        public void Initialize()
        {
            if (!this.initialized)
            {
                GL.Instance.GenTextures(1, id);
                TextureTarget target = this.Target;
                GL.Instance.BindTexture((uint)target, id[0]);
                this.Storage.Apply();
                this.BuiltInSampler.Apply();
                //OpenGL.GenerateMipmap((MipmapTarget)((uint)target));// TODO: does this work?
                GL.Instance.BindTexture((uint)this.Target, 0);
                this.initialized = true;
            }
        }

        /// <summary>
        /// setup texture's image data.
        /// </summary>
        public TexStorageBase Storage { get; private set; }

        /// <summary>
        /// setup texture's sampler properties with default built-in sampler object.
        /// </summary>
        public BuiltInSampler BuiltInSampler { get; private set; }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("Target:{0}, Id:{1}", this.Target, this.Id);
        }
    }
}