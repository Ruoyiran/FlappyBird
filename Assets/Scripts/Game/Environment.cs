﻿using UnityEngine;

namespace FlappyBird
{
    public class Environment : MonoBehaviour
    {
        public Rigidbody2D[] bgRigidbodys;
        public ColumnPool columnPool;
        private float scrollSpeed = -2f;
        private int _envImageWidth;
        private int _envImageHeight;

        private void Start()
        {
            _envImageWidth = Screen.width;
            _envImageHeight = Screen.height;
        }
        private int index = 0;
        private void Update()
        {
            GameState gameState = GameManager.Instance.CurrentGameState;
            switch (gameState)
            {
                case GameState.Ready:
                    ScrollBackgrounds();
                    break;
                case GameState.Playing:
                    ScrollBackgrounds();
                    ScrollAllColumns();
                    break;
                case GameState.GameOver:
                    StopScrollBackgrounds();
                    StopScrollAllColumns();
                    break;
                default:
                    break;
            }
        }

        public void SetScrollSpeed(float speed)
        {
            scrollSpeed = speed;
        }

        private void ScrollBackgrounds()
        {
            for (int i = 0; i < bgRigidbodys.Length; i++)
            {
                bgRigidbodys[i].velocity = new Vector2(scrollSpeed, 0);
            }
        }

        private void ScrollAllColumns()
        {
            columnPool.ScrollAllColumns(scrollSpeed);
        }

        private void StopScrollBackgrounds()
        {
            for (int i = 0; i < bgRigidbodys.Length; i++)
            {
                bgRigidbodys[i].velocity = Vector2.zero;
            }
        }

        private void StopScrollAllColumns()
        {
            columnPool.StopScrollAllColumns();
        }

        public void Stop()
        {
            StopScrollBackgrounds();
            StopScrollAllColumns();
        }

        public void Reset()
        {
            columnPool.ResetPos();
        }

        public void SetMaxColumnGapY(float gapY)
        {
            columnPool.SetMaxColumnGapY(gapY);
        }

        public byte[] GetEnvironmentImageBytes()
        {
            Texture2D tex = ImageTool.RenderToTex(Camera.main, _envImageWidth, _envImageHeight);
            byte[] imageBytes = tex.EncodeToPNG();
            DestroyImmediate(tex);
            Resources.UnloadUnusedAssets();
            return imageBytes;
        }
    }
}