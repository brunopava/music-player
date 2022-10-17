using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIPopup : MonoBehaviour
{
	public Text popupTitle;

	public Button closeButton;
	public Button confirmButton;
	public Button cancelButton;

	public delegate void OnClose();
	public delegate void OnConfirm();
	public delegate void OnCancel();

	public OnClose baseOnClose;
	public OnConfirm baseOnConfirm;
	public OnCancel baseOnCancel;

	public void OpenAnimation()
	{
		GetComponent<CanvasGroup>().DOFade(0,0);
		GetComponent<CanvasGroup>().DOFade(1f,0.5f);
	}

	public void CloseAnimation()
	{
		GetComponent<CanvasGroup>().DOFade(0,0.5f).OnComplete(()=>{
			gameObject.SetActive(false);
		});
	}

	public void OnEnable()
	{
		OpenAnimation();
	}

	public void SetPopupTitle(string title)
    {
    	if(popupTitle != null)
    	{
    	    popupTitle.text = title;
    	}
    }

	public void Initialize(OnClose onClose = null, OnConfirm onConfirm = null, OnCancel onCancel = null)
	{
		baseOnClose = onClose;
		baseOnCancel = onCancel;
		baseOnConfirm = onConfirm;

		if(closeButton != null)
		{
			closeButton.onClick.AddListener(() => {
				if(onClose != null)
				{
					onClose();
					RemoveAllEventListeners();
					CloseAnimation();
				}
			});
		}

		if(confirmButton != null)
		{
			confirmButton.onClick.AddListener(() => {
				if(onConfirm != null)
				{
					onConfirm();	
					RemoveAllEventListeners();
					CloseAnimation();
				}
			});
		}

		if(cancelButton != null)
		{
			cancelButton.onClick.AddListener(() => {
				if(onCancel != null)
				{
					onCancel();	
					RemoveAllEventListeners();
					CloseAnimation();
				}
			});		
		}
	}

	public void RemoveAllEventListeners()
	{
		if(closeButton != null)
		{
			closeButton.onClick.RemoveAllListeners();
		}

		if(confirmButton != null)
		{
			confirmButton.onClick.RemoveAllListeners();
		}

		if(cancelButton != null)
		{
			cancelButton.onClick.RemoveAllListeners();
		}
	}    
}
