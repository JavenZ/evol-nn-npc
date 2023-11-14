extends Node
class_name HealthComponent

@export var healthbar_component : HealthBarComponent
@export var health: float = 100.0
@export var max_health: float = 100.0
@export var damage_cooldown : float = 0.0
@export var dead : bool = false
@export var invincible : bool = false

func _ready():
	# init health bar
	self.update_health_bar()
	
	# reset invincibility state
	# $DamageCooldownTimer.start(self.damage_cooldown) ?
	self.invincible = false

func update_health_bar() -> void:
	self.healthbar_component.update(self.max_health, self.health)

func take_damage(amount: float) -> void:
	"""
	Applies damage to character then updates healthbar and death state.
	"""
	# is player already dead?
	if self.dead:
		return
	
	# apply damage?
	if not self.invincible and amount > 0.0:
		print(self.name, " taking damage: ", amount)
		# subtract health
		self.health = maxf(0.0, self.health - amount)
		
		# begin cooldown timer
		$DamageCooldownTimer.start(self.damage_cooldown)
		self.invincible = true
	
		# health bar display
		self.update_health_bar()
	
	# death? ignores invincibility if character has no health
	if self.health <= 0.0:
		print(self.name, " died.")
		self.dead = true

func _on_damage_cooldown_timer_timeout():
	print(self.name, " damage cooldown expired.")
	self.invincible = false
