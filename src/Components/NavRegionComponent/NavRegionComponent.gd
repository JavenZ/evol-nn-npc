extends NavigationRegion2D
class_name NavRegionComponent

@export var map_component : MapComponent

func calculate_navigation_polys():
	var nav_poly = self.navigation_polygon
	
	# calculate inner map polygon & wall polygons
	var map_poly = polygon_map_to_local(Geometry2D.convex_hull(self.map_boundaries))
	var wall_polys = self.calculate_convex_hulls(self.map_walls)
	print("\t\twalls #: ", len(wall_polys))
	
	# subtract wall polygons from map polygon & update navigation polygons
	nav_poly.add_outline(map_poly)
	for wall_poly in wall_polys:
		var clipped = Geometry2D.clip_polygons(map_poly, polygon_map_to_local(wall_poly))
		for clip in clipped:
			if Geometry2D.is_polygon_clockwise(clip):
				nav_poly.add_outline(clip)
		nav_poly.make_polygons_from_outlines()
	print("\t\tNav poly outlines: ", nav_poly.get_outline_count())
	print("\t\tNav poly polygons: ", nav_poly.get_polygon_count())

func merge_polys(polys):
	var cum_poly := []
	var other_polys := []
	for p in polys:
		var merged = Geometry2D.merge_polygons(cum_poly, p)
		cum_poly = merged[0]
		if len(merged) > 1:
			other_polys.push_back(merged[1])
	return cum_poly

func clip_polys(init_poly, polys):
	var outlines = []
	var cum_poly = init_poly
	for p in polys:
		var merged = Geometry2D.clip_polygons(cum_poly, polygon_map_to_local(p))
		cum_poly = merged[0]
		if len(merged) > 1 and !Geometry2D.is_polygon_clockwise(merged[1]):
			outlines.append(merged[1])
	return outlines

func polys_overlap(poly_a, poly_b) -> bool:
	for p in poly_a:
		if Geometry2D.is_point_in_polygon(p, poly_b):
			return true
	return false

func calculate_convex_hulls(cells):
	var cell_grid = {}
	var islands = []
	
	# init cell grid
	for cell in cells:
		cell_grid[cell] = true
	
	# search for islands
	for cell in cell_grid:
		if cell_grid[cell] != false:
			var island = self.floodfill(cell_grid, cell)
			islands.append(island)
	
	# calculate convex hull polygons for each island
	var hulls = []
	for island in islands:
		var poly = PackedVector2Array(island)
		hulls.append(Geometry2D.convex_hull(poly))
	
	# sort by largest island
	hulls.sort_custom(self.compare_polygons)
	
	return hulls

func floodfill(grid, cell: Vector2i):
	var queue = []
	var region = []
	
	queue.push_back(cell)
	grid[cell] = false
	
	while not queue.is_empty():
		var cur_cell = queue.pop_front()
		
		# Add the element to the output region.
		region.append(cur_cell)
		
		for dir in [Vector2i(1, 0), Vector2i(-1, 0), Vector2i(0, 1), Vector2i(0, -1)]:
			var adj_cell = cur_cell + dir
			if adj_cell in grid and grid[adj_cell] != false:
				# Then add the position to the queue to be visited later
				queue.push_back(adj_cell)
				# And mark this position as visited.
				grid[adj_cell] = false
			
	# there are no more positions to visit
	return region

func compare_polygons(a, b):
	return len(a) > len(b)

func polygon_map_to_local(poly: PackedVector2Array) -> PackedVector2Array:
	var local = []
	for v in poly:
		local.append(self.map_component.map_to_local(v))
	return PackedVector2Array(local)
