extends NavigationAgent2D
class_name NavigationComponent

@export var nav_cooldown : float = 0.25
var freeze : bool = false

func update_target(target: Vector2) -> bool:
	# var initial_call = self.target_position == null
	if !self.freeze:
		# update target navigation positional vector
		self.target_position = target
		
		# cooldown?
		if nav_cooldown > 0.0:
			$CooldownTimer.start(nav_cooldown)
			self.freeze = true
	# return not initial_call
	return true

func finished() -> bool:
	return self.is_navigation_finished()

func next() -> Vector2:
	return self.get_next_path_position()

func _on_cooldown_timer_timeout():
	self.freeze = false

