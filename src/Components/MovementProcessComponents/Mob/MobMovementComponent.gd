extends "res://Components/MovementProcessComponents/MovementProcess.gd"
class_name MobMovementComponent

@export var nav_component : NavigationComponent
@export var detection_component : DetectionComponent
@export var tilemap_component : TileMapComponent
@export var character_component : CharacterComponent
@export var jump_threshold : float = -16.0
@export var turn_threshold : float = 5.0
var jump_freeze : bool = false

func process_movement() -> Dictionary:
	"""
	Implementation function for processing mob movement.
	"""
	# init input vars
	var x : int = 0
	var y : int = 0
	var just_jump : bool = false
	
	# update navigation target
	var body_detected = self.detection_component.body_detected
	if body_detected:
		self.nav_component.update_target(body_detected.position, Util.rand_float(0.5, 1.0))
	else:
		self.nav_component.update_target(self.tilemap_component.get_random_tile(1), Util.rand_float(1.5, 3.5))
	
	# determine next (x, y) movement
	if !self.nav_component.finished():
		var current_agent_position: Vector2 = self.character_component.global_position
		var next_path_position: Vector2 = self.nav_component.next()
		var next_path_dist: Vector2 = next_path_position - current_agent_position
		
		# horizontal
		if abs(next_path_dist.x) > self.turn_threshold:
			x = 1 if next_path_dist.x > 0.0 else -1
		
		# vertical
		y = 1 if next_path_dist.y < self.jump_threshold else -1
		if y == 1 and !self.jump_freeze:
			just_jump = true
			self.jump_freeze = true
			$JumpCooldownTimer.start(Util.rand_float(1.0, 1.5))
		
	# form movement dictionary
	return {
		"x": x,
		"y": y,
		"just_jump": just_jump,
		"released_jump": false,
	}

func _on_jump_cooldown_timer_timeout():
	self.jump_freeze = false
