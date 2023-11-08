extends TileMap
class_name TileMapComponent

func get_random_tile(layer_id: int):
	var tile = Util.choose(self.get_used_cells(layer_id))
	return self.map_to_local(tile)
