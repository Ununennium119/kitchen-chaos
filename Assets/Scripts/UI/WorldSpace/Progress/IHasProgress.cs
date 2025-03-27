using System;

namespace UI.WorldSpace.Progress {
    /// <summary>
    /// This interface should be implemented by the game objects that have progress.
    /// </summary>
    ///
    /// <seealso cref="ProgressBarUI"/>
    public interface IHasProgress {
        /// <summary>
        /// This event is invoked whenever progress of the game object changes.
        /// </summary>
        public event EventHandler<OnProgressChangedArgs> OnProgressChanged;

        public class OnProgressChangedArgs : EventArgs {
            public float ProgressNormalized;
        }
    }
}
