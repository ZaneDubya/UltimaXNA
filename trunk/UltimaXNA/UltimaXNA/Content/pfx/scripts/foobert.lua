
print("my script")

params = util:ParticleEmitterParameters()

params.EmittedMovementDirectionStart = RandomizedVector3(util:Vector3(1, 0, 0))
params.EmittedMovementDirectionEnd = RandomizedVector3(util:Vector3(0, 1, 0))

params.SelfMovementDirectionStart = RandomizedVector3.Invalid;
params.SelfMovementDirectionEnd = RandomizedVector3.Invalid;

params.SelfLifetime = RandomizedValue(5.0)
params.EmittedLifetime = 7

params.EmittedColorStart = RandomizedColor(1, 0, 0, 1)
params.EmittedColorEnd = RandomizedColor(0, 0, 1, 1)

params.PositionProvider = PPCircleOutline(25, false, 3.141592/4.0, 3.141592/4.0)

emitter = util:ParticleEmitter(params)

effect = util:ParticleEffect(0, 0, 0)

function delayedFunc(args)
	util:Log("delayed func!")
	args.em.Parameters.EmittedColorStart = RandomizedColor(1, 1, 1, 1)
	args.em.Parameters.EmittedColorEnd = RandomizedColor(0, 0, 1, 0.2)

	args.em:UpdateShaderParams()
end

function myOnDisposed(emitter)
	util:Log("myOnDisposed")
end

function onStateChanged(emitter, state)
	util:Log(state)
end

function onEffectDisposed(effect)
	util:Log("effect disposed")
end


effect:AddEmitter(emitter)

params.SelfStartPositionOffset = util:Vector3(200, 200, 100)

-- params.LuaOnDisposed = myOnDisposed
-- params.LuaOnStateChanged = onStateChanged

emitter = util:ParticleEmitter(params)

effect:AddEmitter(emitter)

effect.LuaOnDisposed = onEffectDisposed

args = { em = emitter }

-- util:DelayedCall(2000, delayedFunc, args)

