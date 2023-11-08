extends "res://Components/DamageProcessComponents/DamageProcess.gd"

@export var hitbox_component : HitBoxComponent

func process_damage() -> float:
	"""
	Callback function to handle Player damage by frame.
	"""
	# check for enemies inside of player
	var total_dmg = 0.0
	for enemy in self.hitbox_component.inside_bodies:
		total_dmg += 5.0  # impl enemy dmg
	return total_dmg
