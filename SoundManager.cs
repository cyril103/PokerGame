using System;
using System.IO;
using System.Media;
using System.Threading.Tasks;
using System.Windows;

namespace PokerGame
{
    public class SoundManager
    {
        private const string SOUND_FOLDER = "Assets/Sounds";
        
        // Cache players if needed, or just create on fly. 
        // SoundPlayer is lightweight enough for this scale.

        public SoundManager()
        {
            // Ensure directory exists if we were to write, but we are just reading.
        }

        public void PlayClick()
        {
            PlaySound("click.wav", 800, 50);
        }

        public void PlayDeal()
        {
            PlaySound("deal.wav", 600, 100);
        }

        public void PlayWin(HandRank rank)
        {
            // Different sounds for big wins could be implemented
            if (rank >= HandRank.Straight)
            {
                PlaySound("bigwin.wav", 1200, 300); // Higher pitch, longer
            }
            else
            {
                PlaySound("win.wav", 1000, 200);
            }
        }

        public void PlayGameOver()
        {
            PlaySound("gameover.wav", 400, 300);
        }

        private void PlaySound(string filename, int fallbackFreq, int fallbackDuration)
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, SOUND_FOLDER, filename);
            
            if (File.Exists(path))
            {
                try
                {
                    using (var player = new SoundPlayer(path))
                    {
                        player.Play();
                    }
                }
                catch (Exception)
                {
                    // Fallback if file is corrupt or unplayable
                    PlayFallback(fallbackFreq, fallbackDuration);
                }
            }
            else
            {
                PlayFallback(fallbackFreq, fallbackDuration);
            }
        }

        private void PlayFallback(int frequency, int duration)
        {
            // Run on background thread to avoid blocking UI
            Task.Run(() => 
            {
                try
                {
                    Console.Beep(frequency, duration);
                }
                catch
                {
                    // Console.Beep might fail on some systems or configs, ignore.
                }
            });
        }
    }
}
