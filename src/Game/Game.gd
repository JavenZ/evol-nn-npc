extends Node2D
class_name Game

signal finished(report)

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
		a_damage_received += node.health_component.max_health - node.health_component.health
	
	var b_damage_received = 0.0
	for node in self.team_b:
		b_damage_received += node.health_component.max_health - node.health_component.health
	
	# calculate damage given
	var a_damage_given = b_damage_received
	var b_damage_given = a_damage_received
	
	# calculate allies defeated
	var a_ally_deaths = len(self.team_a) - a_size
	var b_ally_deaths = len(self.team_b) - b_size
	
	# calculate enemies defeated
	var a_enemy_deaths = b_ally_deaths
	var b_enemy_deaths = a_ally_deaths
	
	# how long was the match?
	var match_total_time = self.calculate_time_lived()
	
	# create match report
	var report = {
		'game': self.name,
		'winner': winner,
		'match_time': match_total_time,
		'a_damage_given': a_damage_given,
		'b_damage_given': b_damage_given,
		'a_damage_received': a_damage_received,
		'b_damage_received': b_damage_received,
		'a_ally_deaths': a_ally_deaths,
		'b_ally_deaths': b_ally_deaths,
		'a_enemy_deaths': a_enemy_deaths,
		'b_enemy_deaths': b_enemy_deaths,
	}
	
	# emit finished signal
	# print(report)
	self.finished.emit(report)

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
