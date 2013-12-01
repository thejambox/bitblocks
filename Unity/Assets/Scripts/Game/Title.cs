using UnityEngine;
using System.Collections;

public class Title : MonoBehaviour
{
    public TextMesh lblAuthor;
    public TextMesh lblTitle;
    public TextMesh lblMusicBy;
    public GameObject quad;
    private static Title Instance;

    private void Awake()
    {
        Instance = this;

        lblAuthor.color = lblAuthor.color.GetTransparent();
        lblTitle.color = lblTitle.color.GetTransparent();
        lblMusicBy.color = lblMusicBy.color.GetTransparent();
    }

    public static void ShowTitle(string author, string name)
    {
        Instance.ShowTitleInternal(author, name);
    }

    private void ShowTitleInternal(string author, string name)
    {
        lblAuthor.text = string.Format("{0}", author);
        lblTitle.text = string.Format("\"{0}\"", name);

        GoTweenConfig config = new GoTweenConfig();

        // fade in

        config.clearProperties();
        config.colorProp("color", lblMusicBy.color.GetOpaque());
        config.setEaseType(GoEaseType.QuadIn);

        Go.to(lblMusicBy, 0.15f, config);

        config.clearProperties();
        config.colorProp("color", lblAuthor.color.GetOpaque());
        config.setEaseType(GoEaseType.QuadIn);

        Go.to(lblAuthor, 0.5f, config);

        if (!string.IsNullOrEmpty(name))
        {
            config.clearProperties();
            config.colorProp("color", lblTitle.color.GetOpaque());
            config.setEaseType(GoEaseType.QuadIn);
            config.setDelay(0.25f);

            Go.to(lblTitle, 0.25f, config);
        }

        // fade out

        config.clearProperties();
        config.colorProp("color", lblMusicBy.color.GetTransparent());
        config.setDelay(1.5f);

        Go.to(lblMusicBy, 0.25f, config);

        config.clearProperties();
        config.colorProp("color", lblAuthor.color.GetTransparent());
        config.setDelay(1.5f);

        Go.to(lblAuthor, 0.25f, config);

        if (!string.IsNullOrEmpty(name))
        {
            config.clearProperties();
            config.colorProp("color", lblTitle.color.GetTransparent());
            config.setDelay(1.5f);

            Go.to(lblTitle, 0.25f, config);
        }

        config.clearProperties();
        config.materialColor(Color.black.GetTransparent());
        config.setDelay(2f);

        Go.to(quad, 0.25f, config);
    }
}
