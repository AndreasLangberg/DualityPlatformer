using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class GameController : MonoBehaviour
{
    private const float SwitchTime = 1f;
    private const float MaxParticleSize = 80f;
    private const float CameraShift = 0.5f;
    
    [SerializeField] private Character Character1;
    [SerializeField] private Character Character2;
    [SerializeField] private SpriteRenderer BackGroundRenderer;
    [SerializeField] private SpriteRenderer BackGroundRenderer2;
    [SerializeField] private SpriteRenderer BackGroundRenderer3;
    
    [SerializeField] private GameObject RopePrefab;
    [SerializeField] private GameObject ParticlePrefab;

    private bool _usingFirstCharacter = true;
    private bool _usingRope;
    private float _switchTimer;

    private GameObject _ropeObject;

    private Transform _cameraTransform;

    private GameObject _switchParticlesObject;
    private ParticleSystem _switchParticlesSystem;
    private bool _queuedPunch;

    private void Start()
    {
        _cameraTransform = Camera.main.transform;
        
        Character1.SwitchToCharacter(true);
        Character2.SwitchToCharacter(false);

        Character1.PunchedOtherCharacter += onCharacterPunched;
        Character2.PunchedOtherCharacter += onCharacterPunched;
    }

    private void Update()
    {
        if (_switchTimer > 0)
        {
            _switchTimer -= Time.deltaTime;
            float brightness = _usingFirstCharacter ? 0 : 255;
            if(_usingFirstCharacter)
                brightness += ((_switchTimer / SwitchTime) * 255);
            else
                brightness -= ((_switchTimer / SwitchTime) * 255);

            var startPosition = _usingFirstCharacter ? Character1.transform.position : Character2.transform.position;
            var endPosition = _usingFirstCharacter ? Character2.transform.position : Character1.transform.position;
            var lerpPosition = Vector2.Lerp(endPosition, startPosition, _switchTimer / SwitchTime);
            lerpPosition.y += 0.05f;
            
            var camPos = _cameraTransform.position;
            camPos.x = lerpPosition.x;
            camPos.y = lerpPosition.y + CameraShift;
            _cameraTransform.position = camPos;

            if (!_switchParticlesObject)
            {
                _switchParticlesObject = Instantiate(ParticlePrefab, startPosition, Quaternion.identity, null);
                _switchParticlesSystem = _switchParticlesObject.GetComponent<ParticleSystem>();
            }

            var particleSize = MaxParticleSize + 5 - Mathf.Abs(- MaxParticleSize + ((_switchTimer / SwitchTime) * MaxParticleSize * 2));
            var shape = _switchParticlesSystem.shape;
            shape.scale = new Vector3(particleSize, particleSize, 1);
            _switchParticlesSystem.Simulate(SwitchTime - _switchTimer);

            _switchParticlesObject.transform.position = lerpPosition;
            
            if (_switchTimer <= 0)
            {
                brightness = _usingFirstCharacter ? 0 : 255;
                if (_switchParticlesObject)
                    Destroy(_switchParticlesObject);
                commitSwitch();
            }

            BackGroundRenderer.color = new Color32(255, (byte)brightness, (byte)brightness, 255);
            BackGroundRenderer2.color = new Color32((byte)(255 - brightness), (byte)brightness, (byte)brightness, 255);
            BackGroundRenderer3.color = new Color32((byte)(255 - brightness), (byte)brightness, (byte)brightness, 255);
        }

        else
        {
            var camPos = _cameraTransform.position;
            camPos.x = _usingFirstCharacter ? Character1.transform.position.x : Character2.transform.position.x;
            camPos.y = (_usingFirstCharacter ? Character1.transform.position.y : Character2.transform.position.y) + CameraShift;
            _cameraTransform.position = camPos;
        }
        
        if (Input.GetKeyDown(KeyCode.Space) && _switchTimer <= 0)
        {
            switchCharacter();
        }
        
        if (Input.GetKeyDown(KeyCode.H) && !_usingRope)
        {
            var inUseCharacterTransform = _usingFirstCharacter ? Character1.transform : Character2.transform;
            var otherCharacterTransform = !_usingFirstCharacter ? Character1.transform : Character2.transform;
            _ropeObject = Instantiate(RopePrefab, inUseCharacterTransform.position, Quaternion.identity, null);
            var ropeController = _ropeObject.GetComponent<RopeController>();
            ropeController.Origin = inUseCharacterTransform;
            ropeController.Destination = otherCharacterTransform;

            _usingRope = true;
        }

        if (_usingRope)
        {
            if (!Input.GetKey(KeyCode.H))
            {
                if(_ropeObject)
                    Destroy(_ropeObject);
                _usingRope = false;
            }
        }
        
        Character1.UsingRope = _usingRope;
        Character2.UsingRope = _usingRope;
    }

    private void onCharacterPunched()
    {
        _queuedPunch = true;
        switchCharacter();
    }

    private void switchCharacter()
    {
        Character1.SwitchToCharacter(false);
        Character2.SwitchToCharacter(false);
        
        _switchTimer = SwitchTime;
    }

    private void commitSwitch()
    {
        _usingFirstCharacter = !_usingFirstCharacter;
        Character1.SwitchToCharacter(_usingFirstCharacter);
        Character2.SwitchToCharacter(!_usingFirstCharacter);

        if (_queuedPunch)
        {
            _queuedPunch = false;
            var direction = !_usingFirstCharacter
                ? Character2.transform.position - Character1.transform.position
                : Character1.transform.position - Character2.transform.position;

            var script = _usingFirstCharacter ? Character1 : Character2;
        
            script.PunchMe(direction.x < 0);
        }
    }
}
