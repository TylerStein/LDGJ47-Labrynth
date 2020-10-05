using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour
{
    public List<AudioSource> tracks;

    public void Start() {
    }

    public void Update() {
        if (Input.GetKeyDown(KeyCode.Alpha8)) {
            if (tracks[0].volume == 0f) {
                PlayTrack(0, 1f);
            } else {
                PauseTrack(0, 1f);
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha9)) {
            if (tracks[1].volume == 0f) {
                PlayTrack(1, 1f);
            } else {
                PauseTrack(1, 1f);
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha0)) {
            if (tracks[1].volume > 0f) {
                ReplaceTrack(1, 0, 1f);
            } else if (tracks[0].volume > 0f) {
                ReplaceTrack(0, 1, 1f);
            }
        }
    }

    public void PlayTrack(int index, float fadeTime = 0f) {
        AssertValidTrackIndex(index);
        Debug.Log($"Fading In Track {index} @ {fadeTime} seconds");
        StartCoroutine(FadeTrackIn(tracks[index], fadeTime));
    }

    public void PauseTrack(int index, float fadeTime = 0f) {
        AssertValidTrackIndex(index);
        Debug.Log($"Fading Out Track {index} @ {fadeTime} seconds");
        StartCoroutine(FadeTrackOut(tracks[index], fadeTime));
    }

    public void ReplaceTrack(int outIndex, int inIndex, float crossFadeTime = 0f) {
        AssertValidTrackIndex(outIndex);
        AssertValidTrackIndex(inIndex);
        Debug.Log($"Crossfading Tracks (out = {outIndex}, in = {inIndex}) @ {crossFadeTime} seconds");
        StartCoroutine(CrossFadeTracks(tracks[outIndex], tracks[inIndex], crossFadeTime));
    }

    public void AssertValidTrackIndex(int index) {
        if (index > tracks.Count || index < 0) throw new UnityException($"Invalid track index: {index}");
    }

    IEnumerator FadeTrackOut(AudioSource target, float time) {
        float initialVolume = target.volume;
        float elapsedTime = 0f;
        while (target.volume > 0f) {
            elapsedTime += Time.deltaTime * time;
            target.volume = Mathf.Lerp(initialVolume, 0f, elapsedTime);
            yield return new WaitForEndOfFrame();
        }
       
        yield return new WaitForEndOfFrame();
    }

    IEnumerator FadeTrackIn(AudioSource target, float time) {
        float playbackSeconds = target.time;

        float initialVolume = target.volume;
        float elapsedTime = 0f;

        while (target.volume < 1f) {
            elapsedTime += Time.deltaTime * time;
            target.volume = Mathf.Lerp(initialVolume, 1f, elapsedTime);
            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForEndOfFrame();
    }

    IEnumerator CrossFadeTracks(AudioSource outTrack, AudioSource inTrack, float time) {
        float outInitialVolume = outTrack.volume;
        float inInitialVolume = inTrack.volume;

        float elapsedTime = 0f;
        while (outTrack.volume < 1f || inTrack.volume < 1f) {
            elapsedTime += Time.deltaTime * time;
            outTrack.volume = Mathf.Lerp(outInitialVolume, 0f, elapsedTime);
            inTrack.volume = Mathf.Lerp(inInitialVolume, 1f, elapsedTime);
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForEndOfFrame();
    }
}
