/*
 * Copyright 2019 Scott Hwang. All Rights Reserved.
 * This code was modified from ExampleAssistantV2.cs 
 * in unity-sdk-4.0.0. This continues to be licensed 
 * under the Apache License, Version 2.0 as noted below.
 */

/**
* Copyright 2018 IBM Corp. All Rights Reserved.
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
*      http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*
*/
#pragma warning disable 0649

using System;
using System.Collections;
using IBM.Cloud.SDK;
using IBM.Cloud.SDK.Authentication.Iam;
using IBM.Cloud.SDK.Utilities;
using IBM.Watson.Assistant.V2;
using IBM.Watson.Assistant.V2.Model;
using UnityEngine;
using UnityEngine.UI;

public class SimpleBot : MonoBehaviour
{
    [SerializeField]
    private WatsonSettings settings;

    private AssistantService Assistant_service;

    private bool createSessionTested = false;

    // 2020/2/15 Changed to protected to be accessible in CommandBot.cs
    protected bool messageTested = false;
    private bool deleteSessionTested = false;
    private string sessionId;

    public string textResponse = String.Empty;

    [SerializeField]
    private InputField targetInputField;

    [SerializeField]
    private Text targetText;

    //Keep track of whether IBM Watson Assistant should process input or is
    //processing input to create a chat response.
    public enum ProcessingStatus { Process, Processing, Idle, Processed };
    private ProcessingStatus chatStatus;

    public enum InputFieldTrigger { onValueChanged, onEndEdit };

    [SerializeField]
    private InputField externalInputField;
    [SerializeField]
    private InputFieldTrigger externalTriggerType;

    private InputField inputField;

    // The output target GameObject to receive text from this gameObject.
    [SerializeField]
    private GameObject targetGameObject;

    protected bool isSearchSkillIntent = false;
    private bool isSearchResponse = false;

    private void Start()
    {
        // Enable TLS 1.2
        //ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;

        // Disable old protocols
        //ServicePointManager.SecurityProtocol &= ~(SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11);

        LogSystem.InstallDefaultReactors();
        Runnable.Run(CreateService());
        chatStatus = ProcessingStatus.Idle;

        // Since coroutines can't return values I use the onValueChanged listener
        // to trigger an action after waiting for an input to arrive.
        // I originally used enums or flags to keep track if a process such
        // as obtaining a chat response from IBM Assistant was still being processed
        // or was finished processing but this was cumbersome.

        if (externalInputField != null)
        {
            if (externalTriggerType == InputFieldTrigger.onEndEdit)
            {
                externalInputField.onEndEdit.AddListener(delegate { Runnable.Run(ProcessChat(externalInputField.text)); });
            }
            else
            {
                externalInputField.onValueChanged.AddListener(delegate { Runnable.Run(ProcessChat(externalInputField.text)); });
            }
        }

        inputField = gameObject.AddComponent<InputField>();
        inputField.textComponent = gameObject.AddComponent<Text>();
        inputField.onValueChanged.AddListener(delegate { Runnable.Run(ProcessChat(inputField.text)); });
    }

    public IEnumerator CreateService()
    {

        if (string.IsNullOrEmpty(settings.Assistant_apikey))
        {
            throw new IBMException("Please provide Watson Assistant IAM ApiKey for the service.");
        }

        //  Create credential and instantiate service
        //            IamAuthenticator authenticator = new IamAuthenticator(apikey: Assistant_apikey, url: serviceUrl);
        IamAuthenticator authenticator = new IamAuthenticator(apikey: settings.Assistant_apikey);

        //  Wait for tokendata
        while (!authenticator.CanAuthenticate())
            yield return null;

        Assistant_service = new AssistantService(settings.versionDate, authenticator);
        if (!string.IsNullOrEmpty(settings.serviceUrl))
        {
            Assistant_service.SetServiceUrl(settings.serviceUrl);
        }

        Assistant_service.CreateSession(OnCreateSession, settings.assistantId);

        while (!createSessionTested)
        {
            yield return null;
        }
    }

    // Get the "welcome" chat reponse from IBM Watson Assistant
    public IEnumerator Welcome()
    {
        Debug.Log("Welcome");
        // Set chat processing status to "Processing"
        chatStatus = ProcessingStatus.Processing;

        while (!createSessionTested)
        {
            yield return null;
        }

        Assistant_service.Message(OnMessage, settings.assistantId, sessionId);
        while (!messageTested)
        {
            messageTested = false;
            yield return null;
        }

        if (!String.IsNullOrEmpty(textResponse))
        {
            // Set status to show chat processing has finished 
            chatStatus = ProcessingStatus.Processed;
        }

    }

    public void GetChatResponse(string chatInput)
    {
        StartCoroutine(ProcessChat(chatInput));
    }

    public IEnumerator ProcessChat(string chatInput)
    {

        // Set status to show that the chat input is being processed.
        chatStatus = ProcessingStatus.Processing;

        if (Assistant_service == null)
        {
            yield return null;
        }
        if (String.IsNullOrEmpty(chatInput))
        {
            yield return null;
        }

        messageTested = false;
        if (isSearchSkillIntent)
        {
            chatInput = '"' + chatInput.Trim() + '"';
            isSearchSkillIntent = false;
        }
        Debug.Log("Processchat: " + chatInput);
        Debug.Log("isSearchSkillIntent: " + isSearchSkillIntent);
        var inputMessage = new MessageInput()
        {
            Text = chatInput,
            Options = new MessageInputOptions()
            {
                ReturnContext = true
            }
        };

        Assistant_service.Message(OnMessage, settings.assistantId, sessionId, input: inputMessage);

        while (!messageTested)
        {
            messageTested = false;
            yield return null;
        }

        if (!String.IsNullOrEmpty(textResponse))
        {
            // Set status to show chat processing has finished.
            chatStatus = ProcessingStatus.Processed;
        }
    }

    private void OnDeleteSession(DetailedResponse<object> response, IBMError error)
    {
        deleteSessionTested = true;
    }

    // 2020/2/15 - changed to protected virtual to allow for inheritance.
    // This is where the returned chat response is set to send as output
    protected virtual void OnMessage(DetailedResponse<MessageResponse> response, IBMError error)
    {
        Debug.Log("response = " + response.Response);
        if (response.Result.Output.Entities.Capacity > 0)
        {
            foreach (RuntimeEntity entity in response.Result.Output.Entities)
            {
                Debug.Log("No Intent. entityType: " + entity.Entity + " , value: " + entity.Value);
                if (entity.Entity == "grade")
                {
                    isSearchSkillIntent = true;
                }
            }
        }
    
        if (response.Result.Output == null || response.Result.Output.Intents == null)
        {
            Debug.Log("No Intent");
        }
        else
        {
            if (response.Result.Output.Intents.Capacity > 0)
            {
                if (response.Result.Output.Intents[0].Intent == "course_recommentation")
                {
                    foreach (RuntimeEntity entity in response.Result.Output.Entities)
                    {
                        Debug.Log("entityType: " + entity.Entity + " , value: " + entity.Value);
                        if(entity.Entity == "grade")
                        {
                            isSearchSkillIntent = true;
                            isSearchResponse = true;
                        }
                    }
                }
                Debug.Log("intent = " + response.Result.Output.Intents[0].Intent);
            }
        }

        
        if (response == null ||
            response.Result == null ||
            response.Result.Output == null ||
            response.Result.Output.Generic == null ||
            response.Result.Output.Generic.Count < 1)
        {
            textResponse = "I don't know how to respond to that.";
        }
        else
        {
            Debug.Log("response type: " + response.Result.Output.Generic[0].ResponseType);
            

            if (response.Result.Output.Generic[0].ResponseType == "search")
            {
                textResponse = response.Result.Output.Generic[0].Header.ToString();
            }  else {
                textResponse = response.Result.Output.Generic[0].Text.ToString();
            }
        }


        if (targetInputField != null)
        {
            if (response.Result.Output.Generic[0].Results != null)
            {
                //targetInputField.text = response.Result.Output.Generic[0].ToString(); 
                targetInputField.text = response.Result.Output.Generic[0].Results[0].Title.ToString() + "\n" + response.Result.Output.Generic[0].Results[0].Body.ToString();
                Debug.Log("Response: " + targetInputField.text);
                isSearchResponse = false;
            }
            else
            {
                targetInputField.text = textResponse;
            }
            Debug.Log("Input Field available!! isSearchResponse: " + isSearchResponse);
        } else
        {
            Debug.Log("Input Field not available!! isSearchResponse: " + isSearchResponse);
        }

        // Check if the target GameObject has an InputField then place text into it.
        if (targetGameObject != null)
        {
            InputField target = targetGameObject.GetComponent<InputField>();
            if (target != null)
            {
                target.text = textResponse;
            }
        }

        Debug.Log(textResponse);
        messageTested = true;
    }

    private void OnCreateSession(DetailedResponse<SessionResponse> response, IBMError error)
    {
        Log.Debug("SimpleBot.OnCreateSession()", "Session: {0}", response.Result.SessionId);
        sessionId = response.Result.SessionId;
        createSessionTested = true;
    }

    public void SetChatStatus(ProcessingStatus status)
    {
        chatStatus = status;
    }

    public ProcessingStatus GetStatus()
    {
        return chatStatus;
    }

    public bool ServiceReady()
    {
        return createSessionTested;
    }

    public string GetResult()
    {
        chatStatus = ProcessingStatus.Idle;
        return textResponse;
    }

}

