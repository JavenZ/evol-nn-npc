extends Node
class_name PlayerBrainComponent

@export var character_component : CharacterComponent

func _physics_process(delta):
	self.character_component.move(self.process_movement(), delta)

func process_movement() -> Dictionary:
	"""
	Implementation function for processing Player movement.
	"""
	return {
		"x": int(Input.is_action_pressed("move_right")) - int(Input.is_action_pressed("move_left")),
		"y": int(Input.is_action_pressed("move_down")) - int(Input.is_action_pressed("move_up")),
		"just_jump": Input.is_action_just_pressed("jump") == true,
		"jump": Input.is_action_pressed("jump") == true,
		"released_jump": Input.is_action_just_released("jump") == true
	}
