
function MoveOnClick(self, button, down)
  if button == MouseButtons.LeftButton then
	if down then
		UI:AttachMouse(self)
	else
		UI:DettachMouse()
	end
  end
end