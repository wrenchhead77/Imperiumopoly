using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Splash : MonoBehaviour
{
    public Image smallLogo;      // Reference to the small logo Image
    public Image medLogo;      // Reference to the med logo Image (Button)
    public Image largeLogo;      // Reference to the large logo Image (Button)
    public float fadeDuration = 2f; // Duration for the fade effect
    public float delayBeforeFade = 2f; // Delay before starting the fade

    private void Start()
    {
        // Start the small logo fade-out process
        // Start the splash screen sequence
        StartCoroutine(SplashSequence());
    }

    private IEnumerator SplashSequence()
    {
        // Fade out the small logo
        yield return StartCoroutine(FadeOutSmallLogo());

        // Fade in the large logo
        yield return StartCoroutine(FadeInMedLogo());
        // Fade in the large logo
        yield return StartCoroutine(FadeInLargeLogo());
    }

    private IEnumerator FadeOutSmallLogo()
    {
        // Wait for the initial delay
        yield return new WaitForSeconds(delayBeforeFade);

        // Gradually fade out the small logo
        float elapsedTime = 0f;
        Color logoColor = smallLogo.color;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            smallLogo.color = new Color(logoColor.r, logoColor.g, logoColor.b, alpha);
            yield return null;
        }
        smallLogo.gameObject.SetActive(false);
    }

    private IEnumerator FadeInMedLogo()
    {
        medLogo.gameObject.SetActive(true);
        Color logoColor = medLogo.color;
        medLogo.color = new Color(logoColor.r, logoColor.g, logoColor.b, 0f);
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
            medLogo.color = new Color(logoColor.r, logoColor.g, logoColor.b, alpha);
            yield return null;
        }
        medLogo.color = new Color(logoColor.r, logoColor.g, logoColor.b, 1f);
        yield return new WaitForSeconds(delayBeforeFade);

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            yield return null;
        }
        medLogo.gameObject.SetActive(false);
    }

    private IEnumerator FadeInLargeLogo()
    {
        largeLogo.gameObject.SetActive(true);
        Color logoColor = largeLogo.color;
        largeLogo.color = new Color(logoColor.r, logoColor.g, logoColor.b, 0f);
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
            largeLogo.color = new Color(logoColor.r, logoColor.g, logoColor.b, alpha);
            yield return null;
        }

        largeLogo.color = new Color(logoColor.r, logoColor.g, logoColor.b, 1f);
    }

    // Called when the large logo is clicked
    public void OnLargeLogoClicked()
    {
        // Load the next scene
        //SceneMgr.Instance.LoadScene(eScene.FrontEnd);
        SceneManager.LoadScene(1);
    }
}
