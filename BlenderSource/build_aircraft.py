import bpy
import math
import os
from mathutils import Vector

output = os.path.join(os.path.dirname(os.path.abspath(__file__)), 'Hangar01Output')
os.makedirs(output, exist_ok=True)
bpy.ops.object.select_all(action='SELECT')
bpy.ops.object.delete(use_global=False)
bpy.context.scene.unit_settings.system = 'METRIC'
bpy.context.scene.unit_settings.scale_length = 1
materials = {}
for name, color in [('Govde', (0.65, 0.035, 0.025, 1)), ('Kanat', (0.91, 0.81, 0.58, 1)), ('Metal', (0.24, 0.28, 0.32, 1)), ('Lastik', (0.025, 0.025, 0.025, 1)), ('Kokpit', (0.11, 0.055, 0.025, 1))]:
    material = bpy.data.materials.new(name)
    material.diffuse_color = color
    materials[name] = material

def finish(obj, name, material):
    obj.name = name
    obj.data.materials.append(materials[material])
    bpy.ops.object.transform_apply(location=False, rotation=True, scale=True)
    return obj

def box(name, position, dimensions, material):
    bpy.ops.mesh.primitive_cube_add(size=1, location=position)
    obj = bpy.context.object
    obj.dimensions = dimensions
    return finish(obj, name, material)

def cylinder(name, position, radius, depth, material, rotation=(0, 0, 0), vertices=12):
    bpy.ops.mesh.primitive_cylinder_add(vertices=vertices, radius=radius, depth=depth, location=position, rotation=rotation)
    return finish(bpy.context.object, name, material)

def rod(name, start, end, radius=0.04):
    direction = Vector(end) - Vector(start)
    obj = cylinder(name, (Vector(start) + Vector(end)) / 2, radius, direction.length, 'Metal', vertices=8)
    obj.rotation_euler = direction.to_track_quat('Z', 'Y').to_euler()
    bpy.context.view_layer.objects.active = obj
    bpy.ops.object.transform_apply(location=False, rotation=True, scale=True)
    return obj

vertices = []
faces = []
sections = [(-3.2, .42, .48), (-2, .45, .55), (-.4, .43, .52), (1.2, .32, .38), (2.4, .17, .22), (3.2, .07, .12)]
for longitudinal, width, height in sections:
    for angle_index in range(8):
        angle = 2 * math.pi * angle_index / 8
        vertices.append((width * math.cos(angle), longitudinal, 1.16 + height * math.sin(angle)))
faces.append(tuple(reversed(range(8))))
for section_index in range(len(sections) - 1):
    for angle_index in range(8):
        next_angle = (angle_index + 1) % 8
        faces.append((section_index*8+angle_index, section_index*8+next_angle, (section_index+1)*8+next_angle, (section_index+1)*8+angle_index))
faces.append(tuple(range(40, 48)))
mesh = bpy.data.meshes.new('GovdeMesh')
mesh.from_pydata(vertices, [], faces)
mesh.update()
body = bpy.data.objects.new('Govde', mesh)
bpy.context.collection.objects.link(body)
body.data.materials.append(materials['Govde'])
for cockpit_index, longitudinal in enumerate((-.25, .85)):
    cutter = cylinder('Kesici', (0, longitudinal, 1.75), .27, .7, 'Kokpit', vertices=16)
    bpy.context.view_layer.objects.active = body
    modifier = body.modifiers.new('AcikKokpit', 'BOOLEAN')
    modifier.operation = 'DIFFERENCE'
    modifier.object = cutter
    bpy.ops.object.modifier_apply(modifier=modifier.name)
    bpy.data.objects.remove(cutter, do_unlink=True)
    cylinder('KokpitTaban', (0, longitudinal, 1.41), .255, .035, 'Kokpit', vertices=16)
    box('Koltuk', (0, longitudinal+.12, 1.49), (.32, .10, .20), 'Kokpit')
box('UstKanat', (0, -1.2, 2.365), (9.74, 1.5, .15), 'Kanat')
box('AltKanat', (0, -.9, .82), (8.5, 1.5, .15), 'Kanat')
for side in (-1, 1):
    for longitudinal in (-1.55, -.35):
        rod('KanatDikme', (side*3.15, longitudinal, .895), (side*3.15, longitudinal-.3, 2.29))
        rod('KabinDikme', (side*.35, longitudinal, 1.53), (side*.8, longitudinal-.3, 2.29))
    cylinder('Tekerlek', (side*.95, -1.7, .35), .35, .15, 'Lastik', (0, math.pi/2, 0))
    cylinder('TekerlekGobek', (side*1.035, -1.7, .35), .13, .02, 'Metal', (0, math.pi/2, 0))
    for longitudinal in (-2.15, -.65):
        rod('InisDikme', (side*.3, longitudinal, .95), (side*.95, -1.7, .35), .055)
box('YatayKuyruk', (0, 2.75, 1.22), (3, .9, .08), 'Kanat')
tail = box('DikeyKuyruk', (0, 2.7, 1.7), (.08, 1, 1), 'Govde')
for vertex in tail.data.vertices:
    if vertex.co.z > 0 and vertex.co.y < 0:
        vertex.co.y += .4
rod('KuyrukKizagi', (0, 2.7, 1), (0, 3.1, .13), .055)
cylinder('Motor', (0, -3.425, 1.16), .55, .45, 'Metal', (math.pi/2, 0, 0), 18)
for engine_index in range(9):
    angle = engine_index * 2 * math.pi / 9
    cylinder('MotorSilindir', (.37*math.cos(angle), -3.68, 1.16+.37*math.sin(angle)), .105, .16, 'Metal', (math.pi/2, 0, 0), 8)
propeller = box('Pervane', (0, -3.9, 1.16), (2.2, .08, .18), 'Metal')
cylinder('PervaneGobek', (0, -3.97, 1.16), .16, .14, 'Metal', (math.pi/2, 0, 0))
bpy.ops.object.select_all(action='DESELECT')
for obj in list(bpy.context.scene.objects):
    if obj != propeller:
        obj.select_set(True)
bpy.context.view_layer.objects.active = body
bpy.ops.object.join()
body.name = 'Nu_D36'
bpy.ops.object.transform_apply(location=False, rotation=True, scale=True)
bpy.ops.object.select_all(action='SELECT')
bpy.ops.wm.save_as_mainfile(filepath=os.path.join(output, 'Nu_D36.blend'))
bpy.ops.export_scene.fbx(filepath=os.path.join(output, 'Nu_D36.fbx'), use_selection=True, object_types={'MESH'}, apply_scale_options='FBX_SCALE_ALL', axis_forward='-Z', axis_up='Y', bake_space_transform=True, add_leaf_bones=False)
bpy.ops.export_scene.gltf(filepath=os.path.join(output, 'Nu_D36.glb'), use_selection=True)
