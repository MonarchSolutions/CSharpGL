﻿using CSharpGL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace IntroductionVideo {
    public partial class Form1 : Form {
        private Scene scene;
        private ActionList actionList;
        //private CubeNode cubeNode;

        public Form1() {
            InitializeComponent();

            // init resources.
            this.Load += FormMain_Load;
            // render event.
            this.winGLCanvas1.OpenGLDraw += winGLCanvas1_OpenGLDraw;
            // resize event.
            this.winGLCanvas1.Resize += winGLCanvas1_Resize;
        }

        private void FormMain_Load(object sender, EventArgs e) {
            var position = new vec3(5, 3, 4) * 0.5f;
            var center = new vec3(0, 0, 0);
            var up = new vec3(0, 1, 0);
            var camera = new Camera(position, center, up, CameraType.Perspective, this.winGLCanvas1.Width, this.winGLCanvas1.Height);
            var scene = new Scene(camera);
            scene.RootNode = GetNodes();
            {
                var light = new DirectionalLight(new vec3(1, 3, -1));
                scene.Lights.Add(light);
            }
            this.scene = scene;

            var list = new ActionList();
            var transformAction = new TransformAction(scene);
            list.Add(transformAction);
            var shadowAction = new ShadowMappingAction(scene);
            list.Add(shadowAction);
            var renderAction = new RenderAction(scene);
            list.Add(renderAction);
            this.actionList = list;

            // uncomment these lines to enable manipualter of camera!
            var manipulater = new FirstPerspectiveManipulater();
            manipulater.BindingMouseButtons = GLMouseButtons.Right;
            manipulater.Bind(camera, this.winGLCanvas1);
        }

        private GroupNode GetNodes() {
            var groupNode = new GroupNode();
            //{
            //    var model = new Sphere(1, 20, 40);
            //    var node = SpherePointNode.Create(model);
            //    node.Name = "0 Point";
            //    (new FormPropertyGrid(node)).Show();
            //    groupNode.Children.Add(node);
            //}
            //{
            //    var model = new Sphere(1, 20, 40);
            //    var node = SphereLineNode.Create(model);
            //    node.Name = "1 Line";
            //    (new FormPropertyGrid(node)).Show();
            //    groupNode.Children.Add(node);
            //}
            //{
            //    var model = new Sphere(1, 20, 40);
            //    var texture = GetTexture();
            //    var node = SphereTextureNode.Create(model, texture);
            //    node.Name = "2 Texture";
            //    (new FormPropertyGrid(node)).Show();
            //    groupNode.Children.Add(node);
            //}
            {
                var model = new Sphere(1, 20, 40);
                var texture = GetTexture();
                var node = ShadowMappingNode.Create(model, Sphere.strPosition, Sphere.strNormal, model.Size);
                node.Name = "3 Light/Shadow";
                (new FormPropertyGrid(node)).Show();
                groupNode.Children.Add(node);
            }
            {
                string folder = System.Windows.Forms.Application.StartupPath;
                string filename = System.IO.Path.Combine(folder, "floor.obj_");
                var parser = new ObjVNFParser(true);
                ObjVNFResult result = parser.Parse(filename);
                if (result.Error != null) {
                    MessageBox.Show(result.Error.ToString());
                }
                else {
                    ObjVNFMesh mesh = result.Mesh;
                    var model = new ObjVNF(mesh);
                    var node = ShadowMappingNode.Create(model, ObjVNF.strPosition, ObjVNF.strNormal, model.GetSize());
                    node.WorldPosition = new vec3(0, -2, 0);
                    node.Color = Color.Green.ToVec3();
                    node.Name = filename;
                    groupNode.Children.Add(node);
                }
            }

            return groupNode;
        }


        private Texture GetTexture() {
            string folder = System.Windows.Forms.Application.StartupPath;
            var bmp = new Bitmap(System.IO.Path.Combine(folder, @"earth.png"));
            TexStorageBase storage = new TexImageBitmap(bmp, GL.GL_RGBA, 1, true);
            var texture = new Texture(storage,
                new TexParameterfv(TexParameter.PropertyName.TextureBorderColor, 1, 0, 0),
                new TexParameteri(TexParameter.PropertyName.TextureWrapS, (int)GL.GL_CLAMP_TO_EDGE),
                new TexParameteri(TexParameter.PropertyName.TextureWrapT, (int)GL.GL_CLAMP_TO_EDGE),
                new TexParameteri(TexParameter.PropertyName.TextureWrapR, (int)GL.GL_CLAMP_TO_EDGE),
                new TexParameteri(TexParameter.PropertyName.TextureMinFilter, (int)GL.GL_LINEAR),
                new TexParameteri(TexParameter.PropertyName.TextureMagFilter, (int)GL.GL_LINEAR));
            texture.Initialize();
            bmp.Dispose();

            return texture;
        }

        private void winGLCanvas1_OpenGLDraw(object sender, PaintEventArgs e) {
            ActionList list = this.actionList;
            if (list != null) {
                vec4 clearColor = this.scene.ClearColor;
                GL.Instance.ClearColor(clearColor.x, clearColor.y, clearColor.z, clearColor.w);
                GL.Instance.Clear(GL.GL_COLOR_BUFFER_BIT | GL.GL_DEPTH_BUFFER_BIT | GL.GL_STENCIL_BUFFER_BIT);

                Viewport viewport = Viewport.GetCurrent();
                list.Act(new ActionParams(viewport));
            }
        }

        void winGLCanvas1_Resize(object sender, EventArgs e) {
            this.scene.Camera.AspectRatio = ((float)this.winGLCanvas1.Width) / ((float)this.winGLCanvas1.Height);
        }

        private List<VideoScript> scriptList = new List<VideoScript>();
        private bool allScriptsDone = true;
        private List<VideoScript>.Enumerator enumerator;
        private VideoScript currentScript;

        private void timer1_Tick(object sender, EventArgs e) {
            //this.cubeNode.RotationAxis = new vec3(0, 1, 0);
            //this.cubeNode.RotationAngle += 7f;
            //this.sphereNode.RotationAxis = new vec3(0, 1, 0);
            //this.sphereNode.RotationAngle += 7f;

            // how script executes.
            if (allScriptsDone) { return; }
            if (currentScript == null) {
                if (enumerator.MoveNext()) {
                    currentScript = enumerator.Current;
                }
                else {
                    allScriptsDone = true;
                }
            }

            if (currentScript != null) {
                if (!currentScript.Execute()) {
                    currentScript = null;
                }
            }
        }
    }
}