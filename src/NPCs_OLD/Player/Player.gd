extends "res://NPCs/Character.gd"
class_name Player

# internal vars
var inside_enemies : Array[Node] = []

func _ready():
	# add player to groups
	add_to_group("Player")
	
	# call parent ready()
	super._ready()

# user inputs
func _get_movement_input() -> Dictionary:
	return {
		"x": int(Input.is_action_pressed("move_right")) - int(Input.is_action_pressed("move_left")),
		"y": int(Input.is_action_pressed("move_down")) - int(Input.is_action_pressed("move_up")),
		"just_jump": Input.is_action_just_pressed("jump") == true,
		"jump": Input.is_action_pressed("jump") == true,
		"released_jump": Input.is_action_just_released("jump") == true
	}

func _process_skill_input() -> void:
	"""
	Handle player skill inputs.
	"""
	pass

func _process_damage() -> void:
	"""
	Callback function to handle Player damage by frame.
	"""
	# check for enemies inside of player
	var total_dmg = 0.0
	for enemy in inside_enemies:
		total_dmg += 5.0  # impl enemy dmg
	super.take_damage(total_dmg)

func _on_hit_box_body_entered(body):
	inside_enemies.append(body)
	print("Player inside of enemies: ", inside_enemies)

func _on_hit_box_body_exited(body):
	inside_enemies.erase(body)
	print("Player inside of enemy: ", inside_enemies)
