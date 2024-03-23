using Raylib_cs;
using SMW_Rewrite.Scripts.UI;

namespace SMW_Rewrite.Scripts {
    internal abstract class Scene {
        private UIElement[] uiElements;
        private List<Texture2D> loadedTexts;

        public void Load() {
            loadedTexts = [];
            uiElements = [];
            OnLoad();
        }
        public void Unload() {
            foreach (var item in uiElements) {
                item.Destroy();
            }
            foreach (var item in loadedTexts) {
                Raylib.UnloadTexture(item);
            }
            OnUnload();
        }
        public void Update() {
            foreach (var item in uiElements) {
                if (item.active) item.Update();
            }
            uiElements = OnUpdate();
        }
        protected virtual void OnLoad() { }
        protected virtual UIElement[] OnUpdate() { return []; }
        protected virtual void OnUnload() { }
    }
}
