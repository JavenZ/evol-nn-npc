extends NavigationAgent2D
class_name NavigationComponent

@export var freeze : bool = false

func update_target(target: Vector2, cooldown: float = 0.0):
	if !self.freeze:
		# update target navigation positional vector
		self.target_position = self.target
		
		# cooldown?
		if cooldown > 0.0:
			$CooldownTimer.start(cooldown)
			self.freeze = true

func finished() -> bool:
	return self.is_navigation_finished()

func next() -> Vector2:
	return self.get_next_path_position()

func _on_cooldown_timer_timeout():
	self.freeze = false
