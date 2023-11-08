extends AnimatedSprite2D
class_name SpriteComponent

@export_enum("Left:-1", "Right:1") var current_direction : int = 1  # asset permanent facing direction
@export_enum("Left:-1", "Right:1") var spawn_direction : int = 1  # sprite spawn facing direction

func _ready():
	# set sprite spawn facing direction
	self.flip(self.spawn_direction * self.current_direction)

func flip(x):
	self.apply_scale(Vector2(x, 1))
	
func set_direction(x_dir: int) -> void:
	"""
	Updates the sprite facing direction.
		1 = right; -1 = left
		(sprite_dir == -1, x_dir == -1) -> scale.x = 1
		(sprite_dir == -1, x_dir == 1) -> scale.x = -1
		(sprite_dir == 1, x_dir == -1) -> scale.x = -1
		(sprite_dir == 1, x_dir == 1) -> scale.x = 1
	"""
	# change sprite facing direction
	if x_dir == 0:
		return
	
	# flip character sprite
	self.flip(self.spawn_direction * x_dir)
	
	# remember facing direction
	self.spawn_direction = x_dir

func update_animation_state(_state: String):
	if _state == "stop":
		self.stop_animation()
	else:
		self.play_animation()
		if _state in self.sprite_frames.get_animation_names():
			self.animation = _state

func play_animation():
	self.play()

func stop_animation():
	self.stop()
