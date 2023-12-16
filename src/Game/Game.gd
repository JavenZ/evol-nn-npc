extends Node2D
class_name Game

signal finished(results: GameResults)

@export var map : Map
@export var team_a : Array
@export var team_b : Array
@export var match_time : float = 60.0

@onready var a_name = self.name + "_TeamA"
@onready var b_name = self.name + "_TeamB"

func _ready():
	for node in self.team_a:
		node.game = self
		self.map.spawn_character(node, "TeamA")
		node.add_to_group(a_name)
		node.connect("death", on_character_death)
	for node in self.team_b:
		node.game = self
		self.map.spawn_character(node, "TeamB")
		node.add_to_group(b_name)
		node.connect("death", on_character_death)
	
	# start match timer
	$MatchTimer.start(self.match_time)

func get_enemy(char: CharacterComponent) -> CharacterComponent:
	if char.team == "TeamA":
		return get_tree().get_first_node_in_group(b_name) as CharacterComponent
	else:
		return get_tree().get_first_node_in_group(a_name) as CharacterComponent

func finish_match():
	# print("Match finished!")
	# who won?
	var a_size = len(get_tree().get_nodes_in_group(a_name))
	var b_size = len(get_tree().get_nodes_in_group(b_name))
	var winner = "Tie"
	if a_size == 0:
		winner = "TeamB"
	elif b_size == 0:
		winner = "TeamA"
	
	# calculate damage received
	var a_damage_received = 0.0
	for node in self.team_a:
		a_damage_received += node.health_component.health / node.health_component.max_health
	a_damage_received = 1.0 - (a_damage_received / len(self.team_a))
	
	var b_damage_received = 0.0
	for node in self.team_b:
		b_damage_received += node.health_component.health / node.health_component.max_health
	b_damage_received = 1.0 - (b_damage_received / len(self.team_b))
	
	# calculate allies defeated
	var a_ally_deaths = len(self.team_a) - a_size
	var b_ally_deaths = len(self.team_b) - b_size
	
	# how long was the match?
	var match_length = self.calculate_time_lived() / self.match_time
	
	# create match report
	var results = GameResults.new()
	results.GameID = self.name
	results.Winner = winner
	results.MatchTime = match_length
	results.TeamADmgReceived = a_damage_received
	results.TeamBDmgReceived = b_damage_received
	results.TeamADeaths = a_ally_deaths
	results.TeamBDeaths = b_ally_deaths
	
	# emit finished signal
	self.finished.emit(results)
	
	# destroy
	queue_free()
	# self.get_parent().call_deferred("free", self)

func on_character_death(character):
	# remove character from scene tree
	self.map.remove_child(character)
	
	# have either teams been defeated?
	var a_size = len(get_tree().get_nodes_in_group(a_name))
	var b_size = len(get_tree().get_nodes_in_group(b_name))
	if a_size == 0 or b_size == 0:
		finish_match()

func _on_match_timer_timeout():
	# match timed out
	finish_match()

func calculate_time_lived() -> float:
	return self.match_time - $MatchTimer.time_left
