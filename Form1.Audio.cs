using System.Media;

namespace SnakeGame
{
    public partial class Form1
    {
        private void PlayRunStartSound()
        {
            PlaySound(SystemSounds.Asterisk);
        }

        private void PlayEatSound()
        {
            PlaySound(SystemSounds.Beep);
        }

        private void PlayPauseSound()
        {
            PlaySound(SystemSounds.Question);
        }

        private void PlayResumeSound()
        {
            PlaySound(SystemSounds.Asterisk);
        }

        private void PlayFinishSound()
        {
            PlaySound(game.Status == GameStatus.Won ? SystemSounds.Asterisk : SystemSounds.Hand);
        }

        private void PlaySound(SystemSound sound)
        {
            if (!useSound || sound == null)
            {
                return;
            }

            try
            {
                sound.Play();
            }
            catch
            {
                // System sounds are optional polish; gameplay should never depend on them.
            }
        }
    }
}
