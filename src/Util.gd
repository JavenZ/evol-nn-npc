extends Node
class_name Util


# the percent chance something happens
static func chance(num):
	randomize()
	return randi() % 100 <= num

# Util.choose(["one", "two"])   returns one or two
static func choose(choices):
	randomize()
	var rand_index = randi() % choices.size()
	return choices[rand_index]

static func rand_int(min_v=null, max_v=null) -> int:
	randomize()
	if min_v or max_v:
		return randi_range(min_v, max_v)
	else:
		return randi()

static func rand_float(min_v: float, max_v: float) -> float:
	randomize()
	return randf_range(min_v, max_v)

static func generate_id() -> String:
	var x = str("ligma", rand_int())
	return str(hash(x))

static func create_random_binary_map(width: int, height: int, w_zero: int):
	var a = []
	for x in range(width):
		a.append([])
		a[x].resize(height)
		for y in range(height):
			if chance(w_zero):
				a[x][y] = 0
			else:
				a[x][y] = 1
	return a

static func print_matrix(m):
	for row in m:
		print(row)
