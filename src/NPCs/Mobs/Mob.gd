extends "res://NPCs/Character.gd"
class_name Mob

# external vars
@export var detection_radius : float = 750.0
@export var player : Player

# internal vars
@onready var navigation_agent: NavigationAgent2D = $NavigationAgent2D
@onready var navigation_cooldown_timer: Timer = $NavigationCooldownTimer

var player_detected : bool = false
var navigation_freeze: bool = false
var jump_freeze: bool = false
const jump_threshold: float = -16.0
const turn_threshold: float = 5.0

func _ready():
	# update detection shape radius
	if detection_radius > 0.0:
		$DetectionArea/DetectionShape.shape.radius = detection_radius
	
	# call parent ready()
	super._ready()

func get_random_tile():
	var tile = Util.choose(tile_map.get_used_cells(1))
	return tile_map.map_to_local(tile)

func update_player_navigation_target() -> void:
	if !self.navigation_freeze:
		# update target navigation position
		self.navigation_agent.target_position = self.player.position
		
		# cooldown
		self.navigation_cooldown_timer.start(Util.rand_float(0.5, 1.0))
		self.navigation_freeze = true

func update_random_navigation_target() -> void:
	if !self.navigation_freeze:
		# update target navigation position
		self.navigation_agent.target_position = self.get_random_tile()
		
		# cooldown
		self.navigation_cooldown_timer.start(Util.rand_float(1.5, 3.5))
		self.navigation_freeze = true

# Mob AI logic
func _get_movement_input() -> Dictionary:
	# init input vars
	var x : int = 0
	var y : int = 0
	var just_jump : bool = false
	
	# update navigation target
	# await get_tree().physics_frame
	if self.player_detected:
		self.update_player_navigation_target()
	else:
		self.update_random_navigation_target()
	
	# determine next (x, y) movement
	if !navigation_agent.is_navigation_finished():
		var current_agent_position: Vector2 = global_position
		var next_path_position: Vector2 = navigation_agent.get_next_path_position()
		var next_path_dist: Vector2 = next_path_position - current_agent_position
		
		# horizontal
		if abs(next_path_dist.x) > self.turn_threshold:
			x = 1 if next_path_dist.x > 0.0 else -1
		
		# vertical
		y = 1 if next_path_dist.y < jump_threshold else -1
		if y == 1 and !jump_freeze:
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

func _on_detection_area_body_entered(_body):
	# uses collision layer 2 to detect Player
	self.player_detected = true
	# print("Player detected: ", self.player_detected)

func _on_detection_area_body_exited(_body):
	# uses collision layer 2 to detect Player
	self.player_detected = false
	# print("Player detected: ", self.player_detected)

func _on_navigation_cooldown_timer_timeout():
	self.navigation_freeze = false

func _on_jump_cooldown_timer_timeout():
	self.jump_freeze = false
